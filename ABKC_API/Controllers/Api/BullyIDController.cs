using System;
using System.Collections.Generic;
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
    [Route("/api/[controller]")]
    public class BullyIDController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IDogRegistrationService _dogRegService;
        private readonly IMapper _autoMapper;

        public BullyIDController(IDogRegistrationService dogRegService, IMapper autoMapper, IABKCUserService abkcUserService) : base(abkcUserService)
        {
            _dogRegService = dogRegService;
            _autoMapper = autoMapper;
        }
        /// <summary>
        /// request a Bully ID be printed for a given dog
        /// </summary>
        /// <param name="dogId">The dog id retrieved from an ABKC Number search</param>
        /// <returns>The new bully id request</returns>
        [HttpPost("RequestBullyId")]
        public async Task<ActionResult<BullyIdRequestDisplayDTO>> RequestBullyId(int dogId)
        {
            try
            {
                //verify bully id request for dog doesn't already exist
                BullyIdRequestModel existing = await _dogRegService.GetBullyIdRequestForDogId(dogId);
                if (existing != null)
                {
                    return BadRequest($"Bully Id Request for {existing.DogInfo.DogName} has already been started with bullyID request id:{existing.Id}");
                }
                UserModel user = await base.GetLoggedInUser();
                BullyIdRequestModel registration = await _dogRegService.CreateBullyIdRequest(dogId, user);
                BullyIdRequestDisplayDTO rtn = _autoMapper.Map<BullyIdRequestDisplayDTO>(registration);
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Will remove a transfer request from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBullyIdRequest(int id)
        {
            try
            {
                bool result = await _dogRegService.DeleteBullyIdRequest(id);
                if (result == false)
                {
                    return NotFound($"Bully ID Request with Id {id} not found to delete.");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting bully id request. {e.Message}");
            }

        }

        /// <summary>
        /// retrieve a Bully Id request by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BullyIdRequestDisplayDTO>> GetBullyIdRequest(int id)
        {
            BullyIdRequestModel found = await _dogRegService.GetBullyIdRequestById(id);
            if (found == null)
            {
                return NotFound($"Bully id request with Id {id} could not be found");
            }
            BullyIdRequestDisplayDTO dto = _autoMapper.Map<BullyIdRequestDisplayDTO>(found);
            // dto.DocumentTypesProvided = _dogRegService.GetPuppyDocsProvided(id);

            return Ok(dto);
        }
    }
}