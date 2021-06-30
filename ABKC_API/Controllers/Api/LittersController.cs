using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs.Pedigree;
using AutoMapper;
using BullITPDF;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{

    //[Authorize]
    [Route("/api/[controller]")]
    [ApiController]//dotnet 2.1 attribute
    public class LittersController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly ILitterService _litterService;
        private readonly IPedigreeService _pedigreeService;
        private readonly ABKCBuilder _abkcBuilder;
        private readonly IMapper _autoMapper;
        private readonly IGeneralRegistrationService _registrationService;

        public LittersController(IABKCUserService userService, ILitterService litterService,
        IPedigreeService pedigreeService, ABKCBuilder abkcBuilder, IMapper autoMapper, IGeneralRegistrationService registrationService) : base(userService)
        {
            _litterService = litterService;
            _pedigreeService = pedigreeService;
            _abkcBuilder = abkcBuilder;
            _autoMapper = autoMapper;
            _registrationService = registrationService;
        }


        [HttpGet("GetLittersCount")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> GetLittersCount() => await _litterService.GetLittersCount();


        #region "Litter Registration"


        [HttpGet("registrations")]
        public async Task<ActionResult<ICollection<LitterRegistrationDisplayDTO>>> GetLitterRegistrations()
        {
            ICollection<LitterRegistrationModel> col = await _litterService.GetAllLitterRegistrations();

            ICollection<LitterRegistrationDisplayDTO> rtn = _autoMapper.Map<ICollection<LitterRegistrationDisplayDTO>>(col);
            return Ok(rtn);
        }


        /// <summary>
        /// retrieve a litter registration by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LitterRegistrationDisplayDTO>> GetLitterRegistration(int id)
        {
            LitterRegistrationModel found = await _litterService.GetLitterRegistrationById(id);
            if (found == null)
            {
                return NotFound($"Registration with Id {id} could not be found");
            }
            LitterRegistrationDisplayDTO dto = _autoMapper.Map<LitterRegistrationDisplayDTO>(found);
            dto.DocumentTypesProvided = _registrationService.GetDocumentTypesProvidedForRegistration(id, found.RegistrationType);
            var curStatus = found.CurrentStatus;
            if (curStatus != null && curStatus.Status == RegistrationStatusEnum.Pending)
                dto.DateSubmitted = curStatus.DateCreated;

            return Ok(dto);
        }

        /// <summary>
        /// With a given sire and dam, a litter registration may begin.
        /// Both are required or the draft request will fail
        /// </summary>
        /// <param name="damId">Id of the Dam</param>
        /// <param name="sireId">Id of the Sire</param>
        /// <returns></returns>
        [HttpPost("StartRegistration")]
        public async Task<ActionResult<LitterRegistrationDisplayDTO>> StartLitterRegistration(int damId, int sireId)
        {
            try
            {
                UserModel curUser = await base.GetLoggedInUser();
                LitterRegistrationModel reg = await _litterService.StartLitterRegistration(sireId, damId, curUser);
                return Ok(_autoMapper.Map<LitterRegistrationDisplayDTO>(reg));
            }
            catch (System.Exception x)
            {

                //track?
                return BadRequest($"There was an error starting litter registration. {x.Message}");
            }
        }
        /// <summary>
        /// Used to update the litter registration.
        /// </summary>
        /// <param name="id">Id of litter registration</param>
        /// <param name="reg">registration data to update with</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<ActionResult<LitterRegistrationDisplayDTO>> UpdateDraft(int id, [FromBody]LitterDraftDTO reg)
        {
            try
            {
                UserModel curUser = await base.GetLoggedInUser();
                LitterRegistrationModel updated = await _litterService.SaveLitterDraft(id, reg, curUser);
                if (updated == null)
                {
                    return NotFound($"No Litter registration for ${id} could be found to update");
                }
                return Ok(_autoMapper.Map<LitterRegistrationDisplayDTO>(updated));
            }
            catch (System.Exception x)
            {

                //track?
                return BadRequest($"There was an error updating litter registration. {x.Message}");
            }
        }
        /// <summary>
        /// Will remove a litter registration from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteRegistration(int id)
        {
            try
            {
                bool result = await _litterService.DeleteRegistration(id);
                if (result == false)
                {
                    return NotFound($"No Litter registration for ${id} could be found to delete");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting litter registration. {e.Message}");
            }

        }
        #endregion

        #region "Litter Report"


        /// <summary>
        /// Produces a Litter Report from a given litter registration
        /// </summary>
        /// <param name="id">Litter Registration Id</param>
        /// <returns>PDF file stream</returns>
        [HttpGet("LitterReportFromRegistration/{id}")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<ActionResult> GenerateLitterReportFromRegistration(int id)
        {
            try
            {
                LitterReportDTO litterReport = await _litterService.BuildLitterReportFromRegistration(id);
                Stream pdfStream = await _abkcBuilder.GenerateLitterReport(litterReport, true);//always want logo
                pdfStream.Position = 0;
                FileStreamResult result = new FileStreamResult(pdfStream, "application/pdf");
                result.FileDownloadName = $"{litterReport.DamName} Litter Report.pdf";

                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        #endregion

    }
}