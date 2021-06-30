using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using AutoMapper;
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
    public class TransfersController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IDogRegistrationService _dogRegService;
        private readonly IMapper _autoMapper;

        public TransfersController(IDogRegistrationService dogRegService, IMapper autoMapper, IABKCUserService abkcUserService) : base(abkcUserService)
        {
            _dogRegService = dogRegService;
            _autoMapper = autoMapper;
        }
        /// <summary>
        /// begins a transfer request for an existing dog
        /// </summary>
        /// <param name="dogId">The dog  id retrieved from an ABKC Number search</param>
        /// <returns>The new transfer request</returns>
        [HttpPost("requests/StartTransferRequest")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> StartTransferRequest(int dogId)
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
                PuppyRegistrationModel registration = await _dogRegService.StartPuppyRegistration(dogId, user, true);
                PuppyRegistrationDisplayDTO rtn = _autoMapper.Map<PuppyRegistrationDisplayDTO>(registration);
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Takes an existing transfer registration and updates details about it
        /// NOTE:ONLY new owner information can be changed
        /// </summary>
        /// <param name="id">Transfer registration id</param>
        /// <param name="reg">registration details to update</param>
        /// <returns></returns>
        [HttpPost("requests/{id}")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> UpdateTransferRegistration(int id, [FromBody]PuppyRegistrationDraftDTO reg)
        {
            try
            {
                UserModel user = await base.GetLoggedInUser();
                PuppyRegistrationModel registration = await _dogRegService.SavePuppyDraft(id, reg, user);
                if (registration == null)
                {
                    return NotFound($"No Transfer request for ${id} could be found to update");
                }
                PuppyRegistrationDisplayDTO rtn = _autoMapper.Map<PuppyRegistrationDisplayDTO>(registration);
                return Ok(rtn);

            }
            catch (Exception x)
            {

                return BadRequest(x.Message);
            }
        }

        /// <summary>
        /// Will remove a transfer request from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("requests/{id}")]
        public async Task<ActionResult<bool>> DeleteTransferRegistration(int id)
        {
            try
            {
                bool result = await _dogRegService.DeletePuppyRegistration(id);
                if (result == false)
                {
                    return NotFound($"No transfer request for ${id} could be found to delete");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting transfer registration. {e.Message}");
            }

        }

        /// <summary>
        /// retrieve a transfer request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("requests/{id}")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> GetRegistration(int id)
        {
            PuppyRegistrationModel found = await _dogRegService.GetPuppyRegistrationByIdAsync(id);
            if (found == null)
            {
                return NotFound($"Registration with Id {id} could not be found");
            }
            PuppyRegistrationDisplayDTO dto = _autoMapper.Map<PuppyRegistrationDisplayDTO>(found);
            dto.DocumentTypesProvided = _dogRegService.GetPuppyDocsProvided(id);

            return Ok(dto);
        }

        [HttpGet("requests")]
        public async Task<ActionResult<PuppyRegistrationDisplayDTO>> GetRegistrations()
        {
            ICollection<PuppyRegistrationModel> found = await _dogRegService.GetAllPuppyRegistrations();
            found = found.Where(r => r.IsTransferRequest).ToList();

            ICollection<PuppyRegistrationDisplayDTO> rtn = _autoMapper.Map<ICollection<PuppyRegistrationDisplayDTO>>(found);
            return Ok(rtn);
        }
    }
}