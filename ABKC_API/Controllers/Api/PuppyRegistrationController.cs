using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using AutoMapper;
using BullITPDF;
using CoreApp.Helpers;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreApp.Controllers.Api
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    [ApiController]
    public class PuppyRegistrationController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IDogRegistrationService _dogRegService;
        private readonly IDogService _dogService;
        private readonly IMapper _automapper;
        private readonly ABKCBuilder _abkcBuilder;

        public PuppyRegistrationController(IABKCUserService userService, IDogRegistrationService dogRegService,
        IDogService dogService, ABKCBuilder abkcBuilder,
        IMapper automapper) : base(userService)
        {
            _dogRegService = dogRegService;
            _dogService = dogService;
            _automapper = automapper;
            _abkcBuilder = abkcBuilder;
        }
        /// <summary>
        /// begins a permanent registration for a puppy
        /// </summary>
        /// <param name="dogId">The dog (puppy) id retrieved from an ABKC Number search</param>
        /// <returns>The new puppy registration</returns>
        [HttpPost("StartPuppyRegistration")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> StartPuppyPermanentRegistration(int dogId)
        {
            try
            {
                //verify puppy registration for dog doesn't already exist
                PuppyRegistrationModel existing = await _dogRegService.GetPuppyRegistrationForDogId(dogId);
                if (existing != null)
                {
                    return BadRequest($"Puppy registration for {existing.DogInfo.DogName} has already been started with puppy registration id:{existing.Id}");
                }
                UserModel user = await base.GetLoggedInUser();
                PuppyRegistrationModel registration = await _dogRegService.StartPuppyRegistration(dogId, user, false);
                PuppyRegistrationDisplayDTO rtn = _automapper.Map<PuppyRegistrationDisplayDTO>(registration);
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Takes an existing puppy registration and updates details about it
        /// NOTE:only color and microchip information about a dog can be updated
        /// </summary>
        /// <param name="id">Puppy registration id</param>
        /// <param name="reg">registration details to update</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> UpdatePuppyRegistration(int id, [FromBody]PuppyRegistrationDraftDTO reg)
        {
            try
            {
                UserModel user = await base.GetLoggedInUser();
                PuppyRegistrationModel registration = await _dogRegService.SavePuppyDraft(id, reg, user);
                if (registration == null)
                {
                    return NotFound($"No Puppy registration for ${id} could be found to update");
                }

                PuppyRegistrationDisplayDTO rtn = _automapper.Map<PuppyRegistrationDisplayDTO>(registration);
                return Ok(rtn);

            }
            catch (Exception x)
            {

                return BadRequest(x.Message);
            }
        }


        /// <summary>
        /// retrieve a puppy registration by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> GetRegistration(int id)
        {
            try
            {
                PuppyRegistrationModel found = await _dogRegService.GetPuppyRegistrationByIdAsync(id);
                if (found == null)
                {
                    return NotFound($"Registration with Id {id} could not be found");
                }
                PuppyRegistrationDisplayDTO dto = _automapper.Map<PuppyRegistrationDisplayDTO>(found);
                dto.DocumentTypesProvided = _dogRegService.GetPuppyDocsProvided(id);

                return Ok(dto);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Will remove a puppy registration from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePuppyRegistration(int id)
        {
            try
            {
                bool result = await _dogRegService.DeletePuppyRegistration(id);
                if (result == false)
                {
                    return NotFound($"No Litter registration for ${id} could be found to delete");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting puppy registration. {e.Message}");
            }

        }


        /// <summary>
        /// Determines if the dog record for the given id (old system) is available to registered as a puppy
        /// </summary>
        /// <param name="id">Original ABKC system id</param>
        /// <returns></returns>
        [HttpGet("dogValidForRegistration/{id}")]
        public async Task<ActionResult<bool>> CheckIfDogIsUnregisteredPuppy(int id)
        {
            try
            {
                CoreDAL.Models.Dogs dog = await _dogService.GetById(id);
                if (dog == null)
                {
                    return BadRequest($"No dog record with id {id} could be found in the old ABKC system");
                }
                return dog.DogName.ToLowerInvariant().Contains("unregistered puppy");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// For a given ABKC Number finds the puppies that match it
        /// </summary>
        /// <param name="abkcNumber">(6 digit string with or w/o),</param>
        /// <returns></returns>
        [HttpGet("searchForPuppyByABKCNumber")]
        public async Task<ActionResult<ICollection<ABKCDogDTO>>> SearchForPuppyByABKCNumber(string abkcNumber)
        {
            try
            {
                IQueryable<CoreDAL.Models.Dogs> qDogs = _dogService.FindByABKCNumberQuery(abkcNumber);
                var matching = await qDogs.Where(d => d.DogName.ToLower().Contains("unregistered puppy")).ToListAsync();
                ICollection<ABKCDogDTO> dtos = _automapper.Map<ICollection<ABKCDogDTO>>(matching);
                if (dtos != null && dtos.Any())
                {
                    foreach (ABKCDogDTO d in dtos)
                    {
                        //get the breed, color, and id from new table if it exists
                        BaseDogModel found = await _dogService.GetDogByOldTableId(d.OriginalTableId, false);
                        if (found != null)
                        {
                            d.BreedId = found.Breed != null ? found.Breed.Id : 0;
                            d.ColorId = found.Color != null ? found.Color.Id : 0;
                            d.Id = found.Id;
                        }
                    }
                }
                return Ok(dtos ?? new List<ABKCDogDTO>());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Produces a Permanent Registration Certificate from a puppy registration
        /// </summary>
        /// <param name="id">Puppy Registration Id</param>
        /// <returns>PDF file stream</returns>
        [HttpGet("PermanentRegistrationCertificate/{id}")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<ActionResult> GeneratePermanentCertificateFromRegistration(int id)
        {
            try
            {
                PuppyRegistrationModel reg = await _dogRegService.GetPuppyRegistrationByIdAsync(id);
                if (reg == null)
                {
                    return NotFound($"Puppy registration with id {id} cannot be found.");
                }
                DogInfoDTO dog = _automapper.Map<DogInfoDTO>(reg.DogInfo);
                Stream pdfStream = await _abkcBuilder.GeneratePermanentRegistration(dog, false);
                pdfStream.Position = 0;
                FileStreamResult result = new FileStreamResult(pdfStream, "application/pdf");
                result.FileDownloadName = $"{dog.DogName} Permanent Registration.pdf";

                return result;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}