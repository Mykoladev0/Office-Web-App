using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using BullITPDF;
using ABKCCommon.Models.DTOs;

namespace CoreApp.Controllers.Api
{
    [Route("/api/[controller]")]
    public class JuniorHandlersController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IJrHandlerService _handlerService;
        private readonly IMapper _autoMapper;
        private readonly ABKCBuilder _abkcBuilder;

        public JuniorHandlersController(IJrHandlerService handlerService, 
        IMapper autoMapper, IABKCUserService abkcUserService, ABKCBuilder abkcBuilder) : base(abkcUserService)
        {
            _handlerService = handlerService;
            _autoMapper = autoMapper;
            _abkcBuilder = abkcBuilder;
        }
        [HttpGet("registrations")]
        public async Task<ICollection<JuniorHandlerRegistrationDTO>> GetAllJuniorHandlerRegistrations()
        {
            ICollection<JuniorHandlerRegistrationModel> handlers = await _handlerService.GetAllJrRegistrations();
            return _autoMapper.Map<ICollection<JuniorHandlerRegistrationDTO>>(handlers);
        }

        [HttpGet("registrations/{id}")]
        public async Task<ActionResult<JuniorHandlerRegistrationDTO>> GetJuniorHandlerRegistration(int id)
        {
            var rtn = await _handlerService.GetJrRegistrationById(id);
            if (rtn == null)
            {
                return NotFound($"Junior Handler request with Id {id} could not be found");
            }
            return Ok(_autoMapper.Map<JuniorHandlerRegistrationDTO>(rtn));
        }

        /// <summary>
        /// Starts a draft jr Handler registration.
        /// At least a first and last name of the jr handler is required to start the registration
        /// </summary>
        /// <param name="registration"></param>
        /// <returns>A reference of the new jr handler registration</returns>
        [HttpPost("registrations/StartRegistration")]
        public async Task<ActionResult<JuniorHandlerRegistrationDTO>> StartJrHandlerRegistration([FromBody]JuniorHandlerRegistrationDTO registration)
        {
            //check basics
            if (registration == null)
            {
                return BadRequest("Jr Handler registration cannot be empty");
            }
            UserModel user = await base.GetLoggedInUser();
            try
            {
                JuniorHandlerRegistrationModel rtn = await _handlerService.StartDraftRegistration(registration, user);

                return Ok(_autoMapper.Map<JuniorHandlerRegistrationDTO>(rtn));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Saves a draft jr Handler registration.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns>A reference of the updating registration</returns>
        [HttpPost("registrations")]
        public async Task<ActionResult<JuniorHandlerRegistrationDTO>> SaveDraft([FromBody]JuniorHandlerRegistrationDTO registration)
        {
            //check basics
            if (registration == null)
            {
                return BadRequest("Jr Handler registration cannot be empty");
            }
            UserModel user = await base.GetLoggedInUser();
            try
            {
                JuniorHandlerRegistrationModel rtn = await _handlerService.SaveDraftJrRegistration(registration, user);
                if (rtn == null)
                {
                    return NotFound($"Junior Handler request with Id {registration.Id} could not be found");
                }
                return Ok(_autoMapper.Map<JuniorHandlerRegistrationDTO>(rtn));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        /// <summary>
        /// Will remove a jr handler registration from the system.
        /// NOTE: This cannot be done once the registration has been submitted for approval.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("registrations/{id}")]
        public async Task<ActionResult<bool>> DeleteRegistration(int id)
        {
            try
            {
                bool result = await _handlerService.DeleteRegistration(id);
                if (result == false)
                {
                    return NotFound($"Junior Handler request with Id {id} could not be found");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Error deleting jr handler registration. {e.Message}");
            }

        }

               /// <summary>
        /// Produces a Jr Handler Certificate PDF for the given Jr Handler Registration Id
        /// </summary>
        /// <param name="id">Jr Handler Registration Id</param>
        /// <param name="includeBackground">Show the Certificate background or just the text</param>
        /// <returns>PDF file stream</returns>
        [HttpGet("CertificateFromRegistration/{id}")]
        [ProducesResponseType(200, Type = typeof(FileStreamResult))]
        public async Task<ActionResult> GenerateJrHandlerCertificateFromIdNumber(int id, bool includeBackground = false)
        {
            JuniorHandlerRegistrationModel found = await _handlerService.GetJrRegistrationById(id);
            if(found == null){
                return NotFound($"Registration with id {id} was not found.");
            }
            // ABKCBuilder builder = new ABKCBuilder();
            JuniorHandlerDTO dto = _autoMapper.Map<JuniorHandlerDTO>(found);
            Stream pdfStream = await _abkcBuilder.GenerateJrHandlerCertificate(dto, includeBackground);
            pdfStream.Position = 0;
            FileStreamResult result = new FileStreamResult(pdfStream, "application/pdf");
            result.FileDownloadName = $"{found.FirstName} {found.LastName} Jr Handler.pdf";


            return result;
        }
    }
}