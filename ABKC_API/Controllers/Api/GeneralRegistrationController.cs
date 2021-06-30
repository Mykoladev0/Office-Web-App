using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using AutoMapper;
using CoreApp.Helpers;
using CoreApp.Interfaces;
using CoreApp.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CoreApp.Controllers.Api
{
    [Route("api/v1/registrations")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    [ApiController]
    public class GeneralRegistrationController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IGeneralRegistrationService _generalRegService;
        private readonly IMapper _automapper;
        private readonly IOktaUserService _oktaService;
        private readonly IRegistrationNotificationService _notificationService;
        private readonly IOwnerService _ownerService;
        private readonly ILitterService _litterService;
        private readonly IDogRegistrationService _dogRegService;
        private readonly IJrHandlerService _handlerService;

        public GeneralRegistrationController(IGeneralRegistrationService generalRegService,
        IMapper automapper, IOktaUserService oktaService, IABKCUserService userService,
        IRegistrationNotificationService notificationService, IOwnerService ownerService,
        ILitterService litterService, IDogRegistrationService dogRegService,
        IJrHandlerService handlerService) : base(userService)
        {
            _generalRegService = generalRegService;
            _automapper = automapper;
            _oktaService = oktaService;
            _notificationService = notificationService;
            _ownerService = ownerService;
            _litterService = litterService;
            _dogRegService = dogRegService;
            _handlerService = handlerService;
        }


        [HttpGet]
        //retrieve all registrations of all types in the system regardless of status
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetAllRegistrations()
        {
            List<IRegistration> allRegs = new List<IRegistration>();
            ICollection<RegistrationModel> pedigrees = await _dogRegService.GetAllPedigreeRegistrations();
            allRegs.AddRange(pedigrees);
            ICollection<LitterRegistrationModel> litters = await _litterService.GetAllLitterRegistrations();
            allRegs.AddRange(litters);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.GetAllPuppyRegistrations();
            allRegs.AddRange(puppies);
            ICollection<JuniorHandlerRegistrationModel> jrHandler = await _handlerService.GetAllJrRegistrations();
            allRegs.AddRange(jrHandler);
            ICollection<BullyIdRequestModel> idRequests = await _dogRegService.GetAllBullyIdRequests();
            allRegs.AddRange(idRequests);

            var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(allRegs);
            string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
            foreach (var r in rtn)
            {
                //todo:support other thumbnails depending on registration type
                r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
            }
            return Ok(rtn);
        }

        /// <summary>
        /// returns all types of registrations that have been submitted but have not yet been approved
        /// </summary>
        /// <returns></returns>
        [HttpGet("pending")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetPendingRegistrations()
        {

            ICollection<IRegistration> pendingRegs = await _generalRegService.GetPendingRegistrations();

            var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(pendingRegs);
            string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
            foreach (var r in rtn)
            {
                //todo:support other thumbnails depending on registration type
                r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
            }
            return Ok(rtn);
        }

        /// <summary>
        /// All registrations of all status and type for a user id (created)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetAllRegistrationsForUser(int userId)
        {
            UserModel user = await _userService.GetUserFromABKCId(userId);
            if (user == null)
            {
                return BadRequest($"User with id {userId} is not found in the system.");
            }
            try
            {
                ICollection<IRegistration> regs = await _generalRegService.GetRegistrationsForUser(user);
                var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(regs);
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    //todo:support other thumbnails depending on registration type
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// All registrations for a user id (created) that are pending approval
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}/pending")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetPendingRegistrationsForUser(int userId)
        {
            UserModel user = await _userService.GetUserFromABKCId(userId);
            if (user == null)
            {
                return BadRequest($"User with id {userId} is not found in the system.");
            }
            try
            {
                ICollection<IRegistration> regs = await _generalRegService.GetPendingRegistrationsForUser(user);
                var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(regs);
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    //todo:support other thumbnails depending on registration type
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        /// All registrations for a user id (created) that were rejected
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}/rejected")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetRejectedRegistrationsForUser(int userId)
        {
            UserModel user = await _userService.GetUserFromABKCId(userId);
            if (user == null)
            {
                return BadRequest($"User with id {userId} is not found in the system.");
            }
            try
            {
                ICollection<IRegistration> regs = await _generalRegService.GetRejectedRegistrationsForUser(user);
                var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(regs);
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    //todo:support other thumbnails depending on registration type
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// All registrations for a user id (created) that are waiting further information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}/waitingdetails")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetWaitingInformationRegistrationsForUser(int userId)
        {
            UserModel user = await _userService.GetUserFromABKCId(userId);
            if (user == null)
            {
                return BadRequest($"User with id {userId} is not found in the system.");
            }
            try
            {
                ICollection<IRegistration> regs = await _generalRegService.GetWaitingInformationRegistrationsForUser(user);
                var rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(regs);
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    //todo:support other thumbnails depending on registration type
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }
                return Ok(rtn);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Returns all representatives with pending registrations
        /// </summary>
        /// <returns></returns>
        [HttpGet("representatives/pending")]
        public async Task<ICollection<RepresentativeDTO>> GetRepresentativesWithPendingRegistrations()
        {
            ICollection<RepresentativeDTO> rtn = new List<RepresentativeDTO>();
            ICollection<UserModel> repUsers = await _userService.GetByRole(SystemRoleEnum.Representative);
            ICollection<RepresentativeDTO> mapped = _automapper.Map<ICollection<RepresentativeDTO>>(repUsers);
            ICollection<Okta.Sdk.IUser> otkaRepUsers = await _oktaService.GetByRole(SystemRoleEnum.Representative);

            foreach (var rep in mapped)
            {
                var found = otkaRepUsers.FirstOrDefault(r => r.Id == rep.OktaId);
                rep.Profile = found?.Profile;
                ICollection<IRegistration> lst = await _generalRegService.GetRegistrationsByRepresentative(rep.Id);
                rep.RegistrationCount = lst.Count();
                rep.PendingRegistrationCount = lst.Where(reg => reg.CurStatus == RegistrationStatusEnum.Pending).Count();
                if (rep.PendingRegistrationCount > 0)
                {
                    rtn.Add(rep);
                }
            }

            return rtn;
        }

        /// <summary>
        /// returns a file stream of the supporting document if it exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <param name="regType">the type of registration to pull the supporting document from</param>
        /// <returns></returns>
        [HttpGet("{id}/supportingDocument")]
        public async Task<ActionResult> GetSupportingDocument(int id, SupportingDocumentTypeEnum documentType, RegistrationTypeEnum regType)
        {
            AttachmentModel attachment = await _generalRegService.GetSupportingDocument(id, documentType, regType);
            if (attachment != null && attachment.Data != null)
            {
                var ms = new MemoryStream(attachment.Data);
                string extension = System.IO.Path.GetExtension(attachment.FileName).Trim('.');
                FileStreamResult result = new FileStreamResult(ms, $"application/{extension}");
                result.FileDownloadName = $"{id}-{documentType.ToString()}.{extension}";
                return result;
            }
            return NotFound();

        }

        /// <summary>
        /// Get full (flattened) dog information for a given registration id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registrationType">Applicable for Permanent, Transfer, and Pedigree Registrations</param>
        /// <returns></returns>
        [HttpGet("{id}/dogInfo")]
        public async Task<ActionResult<DogInfoDTO>> GetRegistrationDogInfo(int id, RegistrationTypeEnum registrationType)
        {
            DogInfoDTO dog = null;
            BaseDogModel found = null;
            switch (registrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    RegistrationModel reg = await _dogRegService.GetPedigreeRegistrationByIdAsync(id);
                    found = reg?.DogInfo;
                    break;
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    PuppyRegistrationModel puppyReg = await _dogRegService.GetPuppyRegistrationByIdAsync(id);
                    found = puppyReg?.DogInfo;
                    break;
                case RegistrationTypeEnum.BullyId:
                    BullyIdRequestModel request = await _dogRegService.GetBullyIdRequestById(id);
                    found = request?.DogInfo;
                    break;
                default:
                    return null;
            }
            if (found == null)
            {
                return NotFound($"Dog for Registration {id} cannot be found");
            }
            _automapper.Map<DogInfoDTO>(found);
            return Ok(dog);
        }

        /// <summary>
        /// Searches by representative login name and returns registrations
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <param name="pendingOnly">only return registrations that are ready for approval</param>
        /// <returns></returns>
        [HttpGet("searchByRepresentative")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> SearchByRep(string searchTxt, bool pendingOnly)
        {

            ICollection<IRegistration> matching = await _generalRegService.SearchRegistrationsByRepresentative(searchTxt);

            ICollection<RegistrationListItemDTO> rtn = null;
            if (pendingOnly)
            {
                matching = matching.Where(r => r.CurStatus == RegistrationStatusEnum.Pending).ToList();
            }
            try
            {
                rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(matching);
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Error with request: {e.Message}");
            }
            return Ok(rtn);
        }


        /// <summary>
        /// Returns registrations for a representative using rep id
        /// </summary>
        /// <param name="id">ID of representative</param>
        /// <param name="pendingOnly">only return registrations that are ready for approval</param>
        /// <returns></returns>
        [HttpGet("getByRepresentative")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetByRepId(int id, bool pendingOnly)
        {
            IQueryable<RegistrationModel> matching = _dogRegService.GetPedigreeRegistrationsByRepresentativeQuery(id);
            matching = matching
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Include(r => r.StatusHistory);

            ICollection<RegistrationListItemDTO> rtn = null;
            if (pendingOnly)
            {
                matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
            }
            try
            {
                var lst = await matching.ToListAsync();
                rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(lst);
                if (!pendingOnly)
                {
                    //need filter because we only care about pending submissions to calculate submitted date
                    matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
                }

                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Error with request: {e.Message}");
            }
            return Ok(rtn);
        }
        /// <summary>
        /// Returns registrations for a owner using owner id
        /// </summary>
        /// <param name="id">ID of owner</param>
        /// <param name="pendingOnly">only return registrations that are ready for approval</param>
        /// <returns></returns>
        [HttpGet("getByOwner")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> GetByOwner(int id, bool pendingOnly)
        {
            IQueryable<RegistrationModel> matching = _dogRegService.GetPedigreeRegistrationsByOwnerQuery(id);
            matching = matching
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Include(r => r.StatusHistory);

            ICollection<RegistrationListItemDTO> rtn = null;
            if (pendingOnly)
            {
                matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
            }
            try
            {
                var lst = await matching.ToListAsync();
                rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(lst);
                if (!pendingOnly)
                {
                    //need filter because we only care about pending submissions to calculate submitted date
                    matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
                }
                // var submittedDates = await matching.Select(r => new { id = r.Id, date = r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().DateModified }).ToListAsync();
                string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
                foreach (var r in rtn)
                {
                    // var found = submittedDates.Where(s => s.id == r.Id).FirstOrDefault();
                    // if (found != null)
                    // {
                    //     r.DateSubmitted = found.date;
                    // }
                    r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
                }

            }
            catch (Exception e)
            {
                return BadRequest($"Error with request: {e.Message}");
            }
            return Ok(rtn);
        }

        /// <summary>
        /// Searches by owner/coowner name lastname and returns registrations
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <param name="pendingOnly">only return registrations that are ready for approval</param>
        /// <returns></returns>
        [HttpGet("searchByOwner")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> SearchByOwner(string searchTxt, bool pendingOnly)
        {
            IQueryable<RegistrationModel> matching = _dogRegService.SearchPedigreeRegistrationsByOwner(searchTxt);
            matching = matching
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Include(r => r.StatusHistory);
            if (pendingOnly)
            {
                matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
            }
            // var submittedDates = await matching.Select(r => new { id = r.Id, date = r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().DateModified }).ToListAsync();
            string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
            ICollection<RegistrationListItemDTO> rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(matching);
            foreach (var r in rtn)
            {
                // var found = submittedDates.Where(s => s.id == r.Id).FirstOrDefault();
                // if (found != null)
                // {
                //     r.DateSubmitted = found.date;
                // }
                r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
            }
            return Ok(rtn);
        }

        /// <summary>
        /// Searches registrations by dog name
        /// </summary>
        /// <param name="searchTxt"></param>
        /// <param name="pendingOnly">only return registrations that are ready for approval</param>
        /// <returns></returns>
        [HttpGet("searchByDogName")]
        public async Task<ActionResult<ICollection<RegistrationListItemDTO>>> SearchByDogName(string searchTxt, bool pendingOnly)
        {
            IQueryable<RegistrationModel> matching = _dogRegService.SearchPedigreeRegistrationsByDogName(searchTxt);
            matching = matching
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Include(r => r.StatusHistory);
            if (pendingOnly)
            {
                matching = matching.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == RegistrationStatusEnum.Pending);
            }
            // var submittedDates = await matching.Select(r => new { id = r.Id, date = r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().DateModified }).ToListAsync();
            string defaultImageStr = await Utilities.GetDefaultDogImageThumbnailString();
            ICollection<RegistrationListItemDTO> rtn = _automapper.Map<ICollection<RegistrationListItemDTO>>(matching);
            foreach (var r in rtn)
            {
                // var found = submittedDates.Where(s => s.id == r.Id).FirstOrDefault();
                // if (found != null)
                // {
                //     r.DateSubmitted = found.date;
                // }
                r.RegistrationThumbnailBase64 = !string.IsNullOrEmpty(r.RegistrationThumbnailBase64) ? r.RegistrationThumbnailBase64 : defaultImageStr;
            }
            return Ok(rtn);
        }


        /// <summary>
        /// Upload a supporting document for a registration
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="documentType">Types Supported: FrontPedigree, BackPedigree, FrontPhoto, SidePhoto</param>
        /// <param name="registrationType">Registration Types Supported: Pedigree=1,Litter=2,JuniorHandler=3,Transfer=4,Puppy=5</param>
        /// <returns></returns>
        [HttpPost("{id}/supportingdocument")]
        public async Task<IActionResult> PostSupportingDocument([FromRoute]int id, IFormFile file, [FromForm]SupportingDocumentTypeEnum documentType, [FromForm]RegistrationTypeEnum registrationType)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File stream was empty, could not process");
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                Byte[] fileBytes = ms.ToArray();
                UserModel user = await base.GetLoggedInUser();
                bool result = await _generalRegService.AddSupportingDocument(id, file.FileName, fileBytes, documentType, registrationType, user);
                return Ok(result);
            }

        }

        /// <summary>
        /// Returns if a registration is valid for submission. If not, a message as to why is provided
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registrationType"></param>
        /// <returns></returns>
        [HttpPost("{id}/validForSubmission")]
        public async Task<ActionResult> IsRegistrationValidForSubmission(int id, RegistrationTypeEnum registrationType)
        {
            try
            {
                bool isValid = await _generalRegService.RegistrationValidForSubmission(id, registrationType);
                return Ok(new { isValid = true });
            }
            catch (InvalidOperationException e)
            {
                return Ok(new { isValid = false, message = e.Message });
            }
        }

        /// <summary>
        /// for a list of registrations, return for each if it is valid for submission
        /// If not, a message as to why is provided
        /// </summary>
        /// <param name="registrations">A collection of registration ids and types to check for validity</param>
        /// <returns></returns>
        [HttpPost("checkRegistrationsForValidity")]
        public async Task<ActionResult> IsRegistrationValidForSubmission([FromBody]ICollection<RegistrationMappingDTO> registrations)
        {
            var rtn = new List<dynamic>();
            if (registrations == null)
            {
                return BadRequest("No registration list provided");
            }
            foreach (RegistrationMappingDTO reg in registrations)
            {
                string errorMsg = "";
                bool isValid = false;
                try
                {
                    isValid = await _generalRegService.RegistrationValidForSubmission(reg.RegistrationId, reg.RegistrationType);
                }
                catch (InvalidOperationException e)
                {
                    errorMsg = e.Message;
                }
                rtn.Add(new { RegistrationId = reg.RegistrationId, RegistrationType = reg.RegistrationType.ToString(), isValid = isValid, message = errorMsg });
            }

            return Ok(rtn);
        }

        /// <summary>
        /// NOTE: Must go through transaction process
        /// see /api/v1/registrations/getPaymentQuote endpoint
        /// submit a previously saved registration for approval.
        /// Moves status into pending for review by office.
        /// </summary>
        /// <param name="id">the id of the draft registration</param>
        /// <param name="registrationType">Registration Types Supported: Pedigree=1,Litter=2,JuniorHandler=3,Transfer=4,Puppy=5</param>
        /// <returns></returns>
        [HttpPost("{id}/submit")]
        [Obsolete]
        public async Task<ActionResult<bool>> SubmitRegistration(int id, RegistrationTypeEnum registrationType)
        {
            return BadRequest("Submitting now happens through the /api/v1/Payment/finalizeTransaction endpoint after the transaction has been approved");
            // UserModel user = await base.GetLoggedInUser();
            // try
            // {
            //     IRegistration reg = await _generalRegService.SubmitRegistration(id, registrationType, user);
            //     return Ok(reg != null);
            // }
            // catch (Exception ex)
            // {
            //     return BadRequest(ex.Message);
            // }
            // //TODO:use hub context to send out message of new registration submission
            // //https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-2.2

        }

        /// <summary>
        /// Administrators and Office workers can Submit a registration with an optional cash payment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registrationType"></param>
        /// <param name="cashPaid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrators, ABKCOffice")]
        [HttpPost("{id}/OfficeSubmit")]
        public async Task<ActionResult<bool>> SubmitByOffice(int id, RegistrationTypeEnum registrationType, double cashPaid = 0)
        {
            UserModel user = await base.GetLoggedInUser();
            try
            {
                TransactionModel transaction = new TransactionModel
                {
                    Amount = cashPaid,
                    TransactionType = TransactionModel.TransactionTypeEnum.Cash,
                    RegistrationCharges = new List<PaymentItemDTO> { new PaymentItemDTO { Amount = cashPaid, RegistrationId = id, RegistrationType = registrationType } }
                };
                IRegistration reg = await _generalRegService.SubmitRegistration(id, registrationType, user, transaction);
                return Ok(reg != null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Administrators and Office workers can Submit and Approve a registration with an optional cash payment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="registrationType"></param>
        /// <param name="cashPaid"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrators, ABKCOffice")]
        [HttpPost("{id}/officeSubmitAndApprove")]
        public async Task<ActionResult<bool>> SubmitAndApproveByOffice(int id, RegistrationTypeEnum registrationType, double cashPaid = 0, string comments = "")
        {
            UserModel user = await base.GetLoggedInUser();
            try
            {
                TransactionModel transaction = new TransactionModel
                {
                    Amount = cashPaid,
                    TransactionType = TransactionModel.TransactionTypeEnum.Cash,
                    RegistrationCharges = new List<PaymentItemDTO> { new PaymentItemDTO { Amount = cashPaid, RegistrationId = id, RegistrationType = registrationType } }
                };
                IRegistration reg = await _generalRegService.SubmitRegistration(id, registrationType, user, transaction);
                reg = await _generalRegService.ApproveRegistration(id, user, comments, registrationType);
                return Ok(reg != null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Admin or ABKC Office users can approve an existing PENDING registration
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comments"></param>
        /// <param name="registrationType">The type of registration to approve</param>
        /// <returns></returns>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<bool>> ApproveRegistration(int id, string comments, RegistrationTypeEnum registrationType)
        {
            UserModel user = await base.GetLoggedInUser();
            try
            {
                IRegistration reg = await _generalRegService.ApproveRegistration(id, user, comments, registrationType);
                return Ok(reg != null);
            }
            catch (Exception e)
            {
                //log exception?
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Admin or ABKC Office users can approve ALL pending registrations of all types for a representative
        /// </summary>
        /// <param name="repId"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        [HttpPost("approvalall/{repId}")]
        [Authorize(Roles = "Administrators, ABKCOffice")]
        public async Task<ActionResult<ICollection<RegistrationResultDTO>>> ApproveAllForRepresentative(int repId, string comments)
        {
            //TODO: change to support all types of registrations
            ICollection<RegistrationResultDTO> rtn = new List<RegistrationResultDTO>();
            //get all registrations for rep that need approval
            ICollection<IRegistration> pending = await _generalRegService.GetRegistrationsByRepresentative(repId);
            ICollection<IRegistration> regsToApprove = pending.Where(reg => reg.CurStatus == RegistrationStatusEnum.Pending).ToList();
            if (regsToApprove != null && regsToApprove.Any())
            {
                UserModel user = await base.GetLoggedInUser();
                foreach (IRegistration registration in regsToApprove)
                {
                    try
                    {
                        IRegistration reg = await _generalRegService.ApproveRegistration(registration.Id, user, comments, registration.RegistrationType);
                        rtn.Add(new RegistrationResultDTO
                        {
                            RegistrationId = reg.Id,
                            RegistrationType = RegistrationTypeEnum.Pedigree,
                            Successful = true
                        });
                    }
                    catch (System.Exception e)
                    {
                        rtn.Add(new RegistrationResultDTO
                        {
                            RegistrationId = registration.Id,
                            RegistrationType = RegistrationTypeEnum.Pedigree,
                            Successful = false,
                            Reason = e.Message
                        });

                    }
                }
                ICollection<int> approvedIds = rtn.Select(r => r.RegistrationId).ToList();
                await _notificationService.RegistrationsApproved(approvedIds, user.Id);

            }
            return Ok(rtn);
        }


        /// <summary>
        /// Will reject a registration and issue a refund. This operation is permanent and cannot be undone.
        /// </summary>
        /// <param name="registrationId">Id of registration</param>
        /// <param name="reasonForRejection">REQUIRED</param>
        /// <param name="registrationType">The type of registration to approve</param>
        [Authorize(Roles = "Administrators, ABKCOffice")]
        /// <returns></returns>
        [HttpPost("reject/{registrationId}")]
        public async Task<ActionResult<bool>> RejectRegistration(int registrationId, string reasonForRejection, RegistrationTypeEnum registrationType)
        {
            if (string.IsNullOrEmpty(reasonForRejection))
            {
                return BadRequest("Reason for rejection must be provided");
            }
            UserModel user = await base.GetLoggedInUser();
            try
            {
                bool result = await _generalRegService.RejectRegistration(registrationId, reasonForRejection, registrationType, user);
                //TODO: Need to issue refund (in reg service or payment service?)
                return Ok(result);
            }
            catch (Exception e)
            {
                //log exception?
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Will push back a registration because more information is needed.
        /// </summary>
        /// <param name="registrationId">Id of registration</param>
        /// <param name="infoNeeded">REQUIRED</param>
        /// <param name="registrationType">The type of registration to request more information</param>
        /// <returns></returns>
        [Authorize(Roles = "Administrators, ABKCOffice")]
        [HttpPost("requestinfo/{registrationId}")]
        public async Task<ActionResult<bool>> RequestMoreInformation(int registrationId, string infoNeeded, RegistrationTypeEnum registrationType)
        {
            if (string.IsNullOrEmpty(infoNeeded))
            {
                return BadRequest("Reason for more information must be provided");
            }
            UserModel user = await base.GetLoggedInUser();
            try
            {
                bool result = await _generalRegService.RequestInformation(registrationId, infoNeeded, registrationType, user);
                return Ok(result);
            }
            catch (Exception e)
            {
                //log exception?
                return BadRequest(e.Message);
            }
        }


        #region "Payment"

        /// <summary>
        /// Generates a registration submission quote for all of the registrations being offered to be submitted
        /// This is the first step of submitting registrations for approval
        /// After payment authorization has been made using Stripe API
        /// use /api/v1/Payment/finalizeTransaction endpoint to complete transaction
        /// NOTE: ABKC Office Workers will use OfficeSubmit endpoint
        /// </summary>
        /// <param name="registrationsToSubmit">A collection of registration ids with registration types to submit to get a quote</param>
        /// <returns></returns>
        [HttpPost("GetPaymentQuote")]
        public async Task<ActionResult<PaymentQuoteDTO>> GetPaymentQuote([FromBody]ICollection<RegistrationSubmitDTO> registrationsToSubmit)
        {
            if (registrationsToSubmit == null || !registrationsToSubmit.Any())
            {
                return BadRequest("No registrations provided to get a quote for submission");
            }
            UserModel user = await base.GetLoggedInUser();
            try
            {
                PaymentQuoteDTO result = await _generalRegService.GenerateSubmitQuote(registrationsToSubmit, user);
                return Ok(result);
            }
            catch (Exception e)
            {
                //log exception?
                return BadRequest(e.Message);
            }
        }


        #endregion


        private async Task<IRegistration> getRegistration(int registrationId, RegistrationTypeEnum registrationType)
        {
            switch (registrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    return await _dogRegService.GetPedigreeRegistrationByIdAsync(registrationId);
                case RegistrationTypeEnum.Puppy:
                // return await _context.PuppyRegistrations.Where(r => r.Id == registrationId).FirstOrDefaultAsync();
                case RegistrationTypeEnum.Litter:
                    return await _litterService.GetLitterRegistrationById(registrationId);

                default:
                    return null;
            }
        }

    }
}
