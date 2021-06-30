using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using ABKCCommon.Models.DTOs.Pedigree;
using AutoMapper;
using BullITPDF;
using CoreApp.Helpers;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    [ApiController]
    public class PedigreeRegistrationController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IDogRegistrationService _registrationService;
        private readonly IMapper _automapper;
        private readonly IPedigreeService _pedigreeService;
        private readonly ABKCBuilder _abkcBuilder;

        public PedigreeRegistrationController(IABKCUserService userService, IDogRegistrationService registrationService,
        IMapper automapper, IPedigreeService pedigreeService, ABKCBuilder abkcBuilder) : base(userService)
        {
            _registrationService = registrationService;
            _automapper = automapper;
            _pedigreeService = pedigreeService;
            _abkcBuilder = abkcBuilder;
        }

        /// <summary>
        /// With a given dog information (minimum dog's name) start a new pedigree registration
        /// NOTE:any owner information associated with dog will be ignored here
        /// </summary>
        /// <param name="dogInfo">A dog model with at least name entered</param>
        /// <returns></returns>
        [HttpPost("StartPedigreeRegistration")]
        public async Task<ActionResult<PedigreeRegistrationDisplayDTO>> StartPedigreeRegistration(BaseDogDTO dogInfo)
        {
            //start a draft for with the given dog information
            //at a minimum, the dog name must be entered prior to starting a draft
            if (dogInfo == null)
            {
                return BadRequest("New Dog information needed to start a pedigree registration");
            }
            if (string.IsNullOrEmpty(dogInfo.DogName))
            {
                return BadRequest("The dog you are starting a pedigree registration for needs at least a name to start");
            }
            UserModel user = await base.GetLoggedInUser();
            RegistrationModel rtn = await _registrationService.StartDraftPedigreeRegistration(dogInfo, user);
            return Ok(_automapper.Map<PedigreeRegistrationDisplayDTO>(rtn));
        }
        /// <summary>
        /// Saves a draft dog pedigree registration. Minimum of Owner or Dog info entered
        /// Owner information updates MUST be supplied at the toplevel. Owner data at the dog level is ignored.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns>A reference of the updating registration</returns>
        [HttpPost]
        public async Task<ActionResult<PedigreeRegistrationDisplayDTO>> SaveDraft(PedigreeRegistrationDraftDTO registration)
        {
            //check basics
            if (registration == null)
            {
                return BadRequest("Pedigree Registration cannot be empty");
            }
            if (registration.DogInfo == null && registration.Owner == null)
            {
                return BadRequest($"To save a draft pedigree registration either dog or owner information must be entered.");
            }
            try
            {
                //get current user
                UserModel user = await base.GetLoggedInUser();
                RegistrationModel rtn = await _registrationService.SaveDraftPedigreeRegistration(registration, user);
                if (rtn == null)
                {
                    throw new InvalidOperationException($"No registration with Id {registration.Id} could be found");

                }
                PedigreeRegistrationDisplayDTO mapped = _automapper.Map<PedigreeRegistrationDisplayDTO>(rtn);
                mapped.DocumentTypesProvided = _registrationService.GetPedigreeDocsProvided(rtn.Id);
                return Ok(mapped);
            }
            catch (Exception e)
            {
                return BadRequest($"There was an error saving the pedigree draft registration. {e.Message}");
            }

        }


        [HttpGet]
        [Obsolete]
        //retrieve all registrations in the system regardless of status
        public ActionResult<ICollection<RegistrationListItemDTO>> GetAllRegistrations()
        {
            return BadRequest("removed pedigree registrations get all because consumers should be using registrations/ endpoint");
        }

        // [HttpGet]
        // //retrieve all registrations in the system regardless of status
        // public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetAllRegistrations()
        // {
        //     ICollection<RegistrationModel> registrations = await _registrationService.GetAllPedigreeRegistrations();

        //     var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(registrations);
        //     string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
        //     foreach (var r in rtn)
        //     {
        //         r.DateSubmitted = registrations.Where(reg => reg.Id == r.Id).First().CurrentStatus?.DateCreated;
        //         r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
        //     }
        //     return Ok(rtn);
        // }

        /// <summary>
        /// retrieve a registration by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedigreeRegistrationDisplayDTO>> GetRegistration(int id)
        {
            RegistrationModel found = await _registrationService.GetPedigreeRegistrationByIdAsync(id);
            if (found == null)
            {
                return NotFound($"Registration with Id {id} could not be found");
            }
            PedigreeRegistrationDisplayDTO dto = _automapper.Map<PedigreeRegistrationDisplayDTO>(found);
            dto.DocumentTypesProvided = _registrationService.GetPedigreeDocsProvided(id);
            // var curStatus = found.CurrentStatus;
            // if (curStatus != null && curStatus.Status == RegistrationStatusEnum.Pending)
            //     dto.DateSubmitted = curStatus.DateCreated;


            return Ok(dto);
        }

        /// <summary>
        /// Will remove a pedigree registration from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePedigreeRegistration(int id)
        {
            try
            {
                bool result = await _registrationService.DeletePedigreeRegistration(id);
                if (result == false)
                {
                    return NotFound($"No pedigree registration for {id} could be found to delete");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting pedigree registration. {e.Message}");
            }

        }

        #region "Pedigree Printing"

        [HttpGet("PedigreeAncestry")]
        [ProducesResponseType(200, Type = typeof(PedigreeDTO))]
        public async Task<ActionResult<PedigreeDTO>> GeneratePedigree(int dogId, bool useNewSystem = false, int ancestryLevel = 6)
        {
            if (ancestryLevel > 10)
            {
                throw new InvalidOperationException("Can not generate a pedigree greater than 10 levels");
            }
            PedigreeDTO data = await _pedigreeService.GeneratePedigreeData(dogId, useNewSystem, ancestryLevel);
            return Ok(data);
        }

        [HttpGet("PedigreeAncestryFromABKCNumber")]
        [ProducesResponseType(200, Type = typeof(PedigreeDTO))]
        public async Task<ActionResult<PedigreeDTO>> GeneratePedigreeFromABKCNumber(string abkcNumber, int ancestryLevel = 6)
        {
            if (ancestryLevel > 10)
            {
                throw new InvalidOperationException("Can not generate a pedigree greater than 10 levels");
            }
            PedigreeDTO data = await _pedigreeService.GeneratePedigreeDataFromABKCNo(abkcNumber, true, ancestryLevel);
            return Ok(data);
        }

        /// <summary>
        /// Produces a Pedigree PDF for the given ABKC Id number
        /// </summary>
        /// <param name="abkcNumber">ABKC #</param>
        /// <param name="ancestryLevel">how many levels deep</param>
        /// <param name="includeBackground">Show the Pedigree background or just the text</param>
        /// <returns>PDF file stream</returns>
        [HttpGet("PedigreePDFFromABKCNumber/{abkcNumber}")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<ActionResult> GeneratePedigreePDFFromABKCNumber(string abkcNumber,
        int ancestryLevel = 6, bool includeBackground = false)
        {
            if (ancestryLevel > 10)
            {
                throw new InvalidOperationException("Can not generate a pedigree greater than 10 levels");
            }
            PedigreeDTO pedigreeData = await _pedigreeService.GeneratePedigreeDataFromABKCNo(abkcNumber, true, ancestryLevel);
            // ABKCBuilder builder = new ABKCBuilder();

            Stream pdfStream = await _abkcBuilder.GeneratePedigree(pedigreeData, includeBackground, true);
            pdfStream.Position = 0;
            FileStreamResult result = new FileStreamResult(pdfStream, "application/pdf");
            result.FileDownloadName = $"{pedigreeData.Name} Pedigree.pdf";

            return result;
        }

        /// <summary>
        /// Produces a Pedigree PDF for the given ABKC Id number
        /// </summary>
        /// <param name="id">Internal Dog Id #</param>
        /// <param name="useNewSystem">dog id comes from new ABKC tables</param>
        /// <param name="ancestryLevel">how many levels deep</param>
        /// <param name="includeBackground">Show the Pedigree background or just the text</param>
        /// <returns>PDF file stream</returns>
        [HttpGet("PedigreePDFFromIdNumber/{id}")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<ActionResult> GeneratePedigreePDFFromIdNumber(int id, bool useNewSystem = false, int ancestryLevel = 6, bool includeBackground = false)
        {
            if (ancestryLevel > 10)
            {
                throw new InvalidOperationException("Can not generate a pedigree greater than 10 levels");
            }
            PedigreeDTO pedigreeData = await _pedigreeService.GeneratePedigreeData(id, useNewSystem, ancestryLevel);
            // ABKCBuilder builder = new ABKCBuilder();

            Stream pdfStream = await _abkcBuilder.GeneratePedigree(pedigreeData, includeBackground);
            pdfStream.Position = 0;
            FileStreamResult result = new FileStreamResult(pdfStream, "application/pdf");
            result.FileDownloadName = $"{pedigreeData.Name} Pedigree.pdf";


            return result;
        }

        #endregion

    }
}