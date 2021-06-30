using System.Threading.Tasks;
using CoreDAL.Interfaces;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using CoreDAL.Models;
using Microsoft.EntityFrameworkCore.Internal;

using CoreDAL.Models.v2.Registrations;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using ABKCCommon.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CoreDAL.Services
{
    public class GeneralRegistrationService : IGeneralRegistrationService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IMapper _autoMapper;
        private readonly IOwnerService _ownerService;
        private readonly IDogService _dogService;
        private readonly IRegistrationNotificationService _notificationService;
        private readonly ILitterService _litterService;
        private readonly IDogRegistrationService _dogRegService;
        private readonly IJrHandlerService _jrHandlerService;
        private readonly IConfiguration _config;
        private readonly FeesModel _registrationFees;

        public GeneralRegistrationService(ABKCOnlineContext context,
        IOwnerService ownerService, IDogService dogService,
        IMapper autoMapper, IRegistrationNotificationService notificationService,
        ILitterService litterService, IDogRegistrationService dogRegService,
        IJrHandlerService jrHandlerService, Microsoft.Extensions.Configuration.IConfiguration config, IOptions<FeesModel> registrationFees)
        {
            _context = context;
            _autoMapper = autoMapper;
            _ownerService = ownerService;
            _dogService = dogService;
            _notificationService = notificationService;
            _litterService = litterService;
            _dogRegService = dogRegService;
            _jrHandlerService = jrHandlerService;
            _config = config;
            _registrationFees = registrationFees.Value;
        }


        public async Task<bool> RegistrationValidForSubmission(int id, RegistrationTypeEnum registrationType)
        {
            IRegistration valid = await RegistrationValid(id, registrationType);
            return valid != null;
        }

        private async Task<IRegistration> RegistrationValid(int registrationId, RegistrationTypeEnum registrationType)
        {
            IRegistration found = null;
            switch (registrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    found = await _dogRegService.CanSubmitPedigreeRegistration(registrationId);
                    if (found == null)
                    {
                        throw new InvalidOperationException($"Pedigree Registration is missing some information and cannot be submitted");
                    }
                    break;
                case RegistrationTypeEnum.Litter:

                    found = await _litterService.CanSubmitRegistration(registrationId);
                    if (found == null)
                    {
                        throw new InvalidOperationException($"Litter Registration is missing some information and cannot be submitted");
                    }
                    break;
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:

                    found = await _dogRegService.CanSubmitPuppyRegistration(registrationId);
                    if (found == null)
                    {
                        throw new InvalidOperationException($"Registration is missing some information and cannot be submitted");
                    }
                    break;
                case RegistrationTypeEnum.JuniorHandler:

                    found = await _jrHandlerService.CanSubmitRegistration(registrationId);
                    if (found == null)
                    {
                        throw new InvalidOperationException($"Junior Handler Registration is missing some information and cannot be submitted");
                    }
                    break;
                case RegistrationTypeEnum.BullyId:

                    found = await _dogRegService.GetBullyIdRequestById(registrationId);
                    if (found == null)
                    {
                        throw new InvalidOperationException($"BullyId Registration {registrationId} is missing");
                    }
                    if (found.CurStatus != RegistrationStatusEnum.Draft)
                    {
                        throw new InvalidOperationException($"BullyId request is not in draft status and cannot be submitted");
                    }
                    break;
            }
            return found;
        }

        public async Task<IRegistration> SubmitRegistration(int registrationId, RegistrationTypeEnum registrationType, UserModel savedBy, TransactionModel transaction)
        {
            IRegistration found = await RegistrationValid(registrationId, registrationType);
            if (found != null)
            {
                if (found.IsInternationalRegistration && found.OvernightRequested)
                {
                    throw new InvalidOperationException("International registrations cannot be processed overnight");
                }
                if (found.RushRequested && found.OvernightRequested)
                {
                    throw new InvalidOperationException("Registrations cannot be both rush and overnight");
                }
                return await completeRegistrationSubmission(found, savedBy, transaction);
            }
            return null;
        }

        private async Task<IRegistration> completeRegistrationSubmission(IRegistration found, UserModel savedBy, TransactionModel transaction)
        {
            found.SetStatus(RegistrationStatusEnum.Pending, savedBy);

            found.AssociatedTransaction = transaction;
            found.SubmittedBy = savedBy;
            await _context.SaveChangesAsync();
            await _notificationService.NewRegistrationSubmitted(found, found.OvernightRequested, found.RushRequested);
            return found;
        }
        public async Task<IRegistration> ApproveRegistration(int id, UserModel user, string comments, RegistrationTypeEnum regType)
        {
            IRegistration found = await getRegistration(id, regType);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration for id {id} could not be found");
            }
            if (found.CurStatus == RegistrationStatusEnum.Approved)
            {
                throw new InvalidOperationException($"Registration for id {id} has already been approved.");
            }
            if (found.CurStatus != RegistrationStatusEnum.Pending)
            {
                throw new InvalidOperationException($"Registration for id {id} is not in pending status, so it cannot be approved.");
            }

            switch (found.RegistrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    try
                    {
                        found = await _dogRegService.ApprovePedigreeRegistration(found as RegistrationModel, user);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    break;
                case RegistrationTypeEnum.Litter:
                    try
                    {
                        found = await _litterService.ApproveLitterRegistration(found as LitterRegistrationModel, user);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    break;
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    try
                    {
                        found = await _dogRegService.ApprovePuppyRegistration(found as PuppyRegistrationModel, user);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    break;
                case RegistrationTypeEnum.JuniorHandler:
                    try
                    {
                        found = await _jrHandlerService.ApproveRegistration(found as JuniorHandlerRegistrationModel, user);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    break;
                case RegistrationTypeEnum.BullyId:
                    try
                    {
                        found = await _dogRegService.ApproveBullyIdRequest(found as BullyIdRequestModel, user);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    break;
            }

            found.SetStatus(RegistrationStatusEnum.Approved, user, comments);
            await _context.SaveChangesAsync();

            //TODO: send out email to owners
            await _notificationService.RegistrationApproved(found);
            return found;
        }

        public async Task<bool> RejectRegistration(int registrationId, string reasonForRejection, RegistrationTypeEnum registrationType, UserModel rejectedBy)
        {
            IRegistration found = await getRegistration(registrationId, registrationType);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} could not be found");
            }
            if (found.CurStatus == RegistrationStatusEnum.Approved)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} has already been approved.");
            }
            if (found.CurStatus != RegistrationStatusEnum.Pending)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} is not in pending status, so it cannot be rejected.");
            }

            found.SetStatus(RegistrationStatusEnum.Denied, rejectedBy, reasonForRejection);
            await _context.SaveChangesAsync();

            //TODO: send out email to person who submitted
            await _notificationService.RegistrationDenied(found);
            return true;
        }

        public async Task<bool> RequestInformation(int registrationId, string infoNeeded, RegistrationTypeEnum regType, UserModel user)
        {
            IRegistration found = await getRegistration(registrationId, regType);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} could not be found");
            }
            if (found.CurStatus == RegistrationStatusEnum.Approved)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} has already been approved.");
            }
            if (found.CurStatus != RegistrationStatusEnum.Pending)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} is not in pending status, so it cannot be sent back for more information.");
            }
            if (string.IsNullOrEmpty(infoNeeded))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} cannot be pushed back without information why.");
            }
            found.SetStatus(RegistrationStatusEnum.WaitingForDetails, user, infoNeeded);
            await _context.SaveChangesAsync();

            //TODO: send out email to person who submitted
            await _notificationService.RegistrationInformationRequested(found);
            return true;
        }

        #region Supporting Documents

        public async Task<bool> AddSupportingDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, RegistrationTypeEnum registrationType, UserModel savedBy)
        {
            bool rtnVal = false;
            switch (registrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    rtnVal = await AddSupportingPedigreeRegistrationDocument(regId, fileName, docStream, documentType, savedBy);
                    break;
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    rtnVal = await AddSupportingPuppyRegistrationDocument(regId, fileName, docStream, documentType, savedBy);
                    break;
                case RegistrationTypeEnum.Litter:
                    rtnVal = await AddSupportingLitterRegistrationDocument(regId, fileName, docStream, documentType, savedBy);
                    break;
                case RegistrationTypeEnum.JuniorHandler:
                    return false;//no supporting docs for jr handler
                case RegistrationTypeEnum.BullyId:
                    rtnVal = await _dogRegService.AddSupportingBullyIdRegistrationDocument(regId, fileName, docStream, documentType, savedBy);
                    break;

                default:
                    rtnVal = false;
                    break;

            }
            if (rtnVal)
            {
                await _context.SaveChangesAsync();
            }

            return rtnVal;
        }

        private async Task<bool> AddSupportingPedigreeRegistrationDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, UserModel savedBy)
        {
            //store documents in DB, when a registration is approved, the documents should move to azure blob storage
            RegistrationModel found = await _context.Registrations.FirstOrDefaultAsync(r => r.Id == regId);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration with Id {regId} could not be found, no supporting document added.");
            }
            AttachmentModel attachment = new AttachmentModel
            {
                Data = docStream,
                FileName = fileName,
            };
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.FrontPedigree:
                    found.FrontPedigree = attachment;
                    break;
                case SupportingDocumentTypeEnum.BackPedigree:
                    found.BackPedigree = attachment;
                    break;
                case SupportingDocumentTypeEnum.FrontPhoto:
                    found.FrontPhoto = attachment;
                    found.RegistrationThumbnailBase64 = GetThumbnailBase64String(attachment);
                    break;
                case SupportingDocumentTypeEnum.SidePhoto:
                    found.SidePhoto = attachment;
                    break;
                case SupportingDocumentTypeEnum.OwnerSignature:
                    found.OwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.CoOwnerSignature:
                    found.CoOwnerSignature = attachment;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private async Task<bool> AddSupportingPuppyRegistrationDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, UserModel savedBy)
        {
            //store documents in DB, when a registration is approved, the documents should move to azure blob storage
            PuppyRegistrationModel found = await _context.PuppyRegistrations.FirstOrDefaultAsync(r => r.Id == regId);
            if (found == null)
            {
                throw new InvalidOperationException($"Puppy Registration with Id {regId} could not be found, no supporting document added.");
            }
            AttachmentModel attachment = new AttachmentModel
            {
                Data = docStream,
                FileName = fileName,
            };
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.OwnerSignature:
                    found.OwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.CoOwnerSignature:
                    found.CoOwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.SellerSignature:
                    found.SellerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.CoSellerSignature:
                    found.CoSellerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.BillOfSaleBack:
                    found.BillOfSaleBack = attachment;
                    break;
                case SupportingDocumentTypeEnum.BillOfSaleFront:
                    found.BillOfSaleFront = attachment;
                    break;
                default:
                    return false;
            }
            return true;
        }

        private async Task<bool> AddSupportingLitterRegistrationDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, UserModel savedBy)
        {
            //store documents in DB, when a registration is approved, the documents should move to azure blob storage
            LitterRegistrationModel found = await _context.LitterRegistrations.FirstOrDefaultAsync(r => r.Id == regId);
            if (found == null)
            {
                throw new InvalidOperationException($"Litter Registration with Id {regId} could not be found, no supporting document added.");
            }
            AttachmentModel attachment = new AttachmentModel
            {
                Data = docStream,
                FileName = fileName,
            };
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.SireOwnerSignature:
                    found.SireOwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.SireCoOwnerSignature:
                    found.SireCoOwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.DamOwnerSignature:
                    found.DamOwnerSignature = attachment;
                    break;
                case SupportingDocumentTypeEnum.DamCoOwnerSignature:
                    found.DamCoOwnerSignature = attachment;
                    break;
                default:
                    return false;
            }
            return true;
        }


        public async Task<AttachmentModel> GetSupportingDocument(int id, SupportingDocumentTypeEnum documentType, RegistrationTypeEnum regType)
        {
            switch (regType)
            {
                case RegistrationTypeEnum.Pedigree:
                    return await _dogRegService.GetSupportingPedigreeDocument(id, documentType);
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    return await _dogRegService.GetSupportingPuppyDocument(id, documentType);
                case RegistrationTypeEnum.Litter:
                    return await _litterService.GetSupportingDocument(id, documentType);
                case RegistrationTypeEnum.BullyId:
                    return await _dogRegService.GetSupportingBullyIdDocument(id, documentType);
                default:
                    return null;
            }

        }

        public ICollection<SupportingDocumentTypeEnum> GetDocumentTypesProvidedForRegistration(int id, RegistrationTypeEnum regType)
        {
            switch (regType)
            {
                case RegistrationTypeEnum.Pedigree:
                    return _dogRegService.GetPedigreeDocsProvided(id);
                case RegistrationTypeEnum.Litter:
                    return _litterService.GetLitterDocsProvided(id);
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    return _dogRegService.GetPuppyDocsProvided(id);
                case RegistrationTypeEnum.JuniorHandler:
                    return _jrHandlerService.GetJrHandlerDocsProvided(id);
                case RegistrationTypeEnum.BullyId:
                    return _dogRegService.GetBullyIdDocsProvided(id);

                default:
                    return new List<SupportingDocumentTypeEnum>();
            }
        }

        #endregion

        public async Task<ICollection<IRegistration>> GetPendingRegistrations()
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            ICollection<RegistrationModel> pendingPedigree = await _dogRegService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending);
            var pendingLitter = await _litterService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending);
            var pendingPuppy = await _dogRegService.GetPuppyRegistrationsByStatus(RegistrationStatusEnum.Pending);
            var pendingJrHandler = await _jrHandlerService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyIdRegistrationsByStatus(RegistrationStatusEnum.Pending);

            pending.AddRange(pendingPedigree);
            pending.AddRange(pendingLitter);
            pending.AddRange(pendingPuppy);
            pending.AddRange(pendingJrHandler);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }

        /// <summary>
        /// returns all registrations of all type for a user by who first saved it
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ICollection<IRegistration>> GetRegistrationsForUser(UserModel user)
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            ICollection<RegistrationModel> pendingPedigree = await _dogRegService.GetRegistrationsByStatus(null, user);
            var pendingLitter = await _litterService.GetRegistrationsByStatus(null, user);
            var pendingPuppy = await _dogRegService.GetPuppyRegistrationsByStatus(null, user);
            var pendingJrHandler = await _jrHandlerService.GetRegistrationsByStatus(null, user);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyIdRegistrationsByStatus(null, user);
            pending.AddRange(pendingPedigree);
            pending.AddRange(pendingLitter);
            pending.AddRange(pendingPuppy);
            pending.AddRange(pendingJrHandler);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }

        public async Task<ICollection<IRegistration>> GetPendingRegistrationsForUser(UserModel user)
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            ICollection<RegistrationModel> pendingPedigree = await _dogRegService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending, user);
            var pendingLitter = await _litterService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending, user);
            var pendingPuppy = await _dogRegService.GetPuppyRegistrationsByStatus(RegistrationStatusEnum.Pending, user);
            var pendingJrHandler = await _jrHandlerService.GetRegistrationsByStatus(RegistrationStatusEnum.Pending, user);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyIdRegistrationsByStatus(RegistrationStatusEnum.Pending, user);
            pending.AddRange(pendingPedigree);
            pending.AddRange(pendingLitter);
            pending.AddRange(pendingPuppy);
            pending.AddRange(pendingJrHandler);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }


        public async Task<ICollection<IRegistration>> GetWaitingInformationRegistrationsForUser(UserModel user)
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            ICollection<RegistrationModel> pendingPedigree = await _dogRegService.GetRegistrationsByStatus(RegistrationStatusEnum.WaitingForDetails, user);
            var pendingLitter = await _litterService.GetRegistrationsByStatus(RegistrationStatusEnum.WaitingForDetails, user);
            var pendingPuppy = await _dogRegService.GetPuppyRegistrationsByStatus(RegistrationStatusEnum.WaitingForDetails, user);
            var pendingJrHandler = await _jrHandlerService.GetRegistrationsByStatus(RegistrationStatusEnum.WaitingForDetails, user);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyIdRegistrationsByStatus(RegistrationStatusEnum.WaitingForDetails, user);
            pending.AddRange(pendingPedigree);
            pending.AddRange(pendingLitter);
            pending.AddRange(pendingPuppy);
            pending.AddRange(pendingJrHandler);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }

        public async Task<ICollection<IRegistration>> GetRejectedRegistrationsForUser(UserModel user)
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            ICollection<RegistrationModel> pendingPedigree = await _dogRegService.GetRegistrationsByStatus(RegistrationStatusEnum.Denied, user);
            var pendingLitter = await _litterService.GetRegistrationsByStatus(RegistrationStatusEnum.Denied, user);
            var pendingPuppy = await _dogRegService.GetPuppyRegistrationsByStatus(RegistrationStatusEnum.Denied, user);
            var pendingJrHandler = await _jrHandlerService.GetRegistrationsByStatus(RegistrationStatusEnum.Denied, user);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyIdRegistrationsByStatus(RegistrationStatusEnum.Denied, user);
            pending.AddRange(pendingPedigree);
            pending.AddRange(pendingLitter);
            pending.AddRange(pendingPuppy);
            pending.AddRange(pendingJrHandler);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }

        public async Task<ICollection<IRegistration>> GetRegistrationsByRepresentative(int repId)
        {
            List<IRegistration> pending = new List<IRegistration>();
            //ask each service to get pending registrations
            var pedQuery = _dogRegService.GetPedigreeRegistrationsByRepresentativeQuery(repId);
            var pedigrees = await pedQuery.ToListAsync();
            ICollection<LitterRegistrationModel> litters = await _litterService.GetLittersByRepresentative(repId);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.GetPuppyRegistrationsByRepresentative(repId);
            ICollection<JuniorHandlerRegistrationModel> handlers = await _jrHandlerService.GetByRepresentative(repId);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyRequestsByRepresentative(repId);
            pending.AddRange(pedigrees);
            pending.AddRange(litters);
            pending.AddRange(puppies);
            pending.AddRange(handlers);
            pending.AddRange(bullyId);

            return sortRegistrations(pending);
        }
        public async Task<ICollection<IRegistration>> GetRegistrationsByOwner(int ownerId)
        {
            List<IRegistration> rtn = new List<IRegistration>();
            //ask each service to get pending registrations
            var pedQuery = _dogRegService.GetPedigreeRegistrationsByOwnerQuery(ownerId);
            var pedigrees = await pedQuery.ToListAsync();
            ICollection<LitterRegistrationModel> litters = await _litterService.GetRegistrationsByOwner(ownerId);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.GetPuppyRegistrationsByOwner(ownerId);
            ICollection<JuniorHandlerRegistrationModel> handlers = await _jrHandlerService.GetRegistrationsByOwner(ownerId);
            ICollection<BullyIdRequestModel> bullyId = await _dogRegService.GetBullyRequestsByOwner(ownerId);
            rtn.AddRange(pedigrees);
            rtn.AddRange(litters);
            rtn.AddRange(puppies);
            rtn.AddRange(handlers);
            rtn.AddRange(bullyId);

            return sortRegistrations(rtn);
        }

        public async Task<ICollection<IRegistration>> SearchRegistrationsByRepresentative(string searchText)
        {
            List<IRegistration> rtn = new List<IRegistration>();
            //ask each service to get pending registrations
            var pedQuery = _dogRegService.SearchPedigreeRegistrationsByRepresentativeQuery(searchText);
            var pedigrees = await pedQuery.ToListAsync();
            ICollection<LitterRegistrationModel> litters = await _litterService.SearchRegistrationsByRepresentative(searchText);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.SearchPuppyRegistrationsByRepresentative(searchText);
            ICollection<JuniorHandlerRegistrationModel> handlers = await _jrHandlerService.SearchRegistrationsByRepresentative(searchText);
            rtn.AddRange(pedigrees);
            rtn.AddRange(litters);
            rtn.AddRange(puppies);
            rtn.AddRange(handlers);
            return sortRegistrations(rtn);

        }
        public async Task<ICollection<IRegistration>> SearchRegistrationsByOwner(string searchText)
        {
            string searchLower = searchText.ToLower();
            List<IRegistration> rtn = new List<IRegistration>();
            //ask each service to get pending registrations
            var pedQuery = _dogRegService.SearchPedigreeRegistrationsByOwner(searchText);
            var pedigrees = await pedQuery.ToListAsync();
            ICollection<LitterRegistrationModel> litters = await _litterService.SearchRegistrationsByOwner(searchText);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.SearchPuppyRegistrationsByOwner(searchText);
            ICollection<JuniorHandlerRegistrationModel> handlers = await _jrHandlerService.SearchRegistrationsByOwner(searchText);
            rtn.AddRange(pedigrees);
            rtn.AddRange(litters);
            rtn.AddRange(puppies);
            rtn.AddRange(handlers);

            return sortRegistrations(rtn);
        }
        public async Task<ICollection<IRegistration>> SearchRegistrationsByDogName(string searchText)
        {
            string searchLower = searchText.ToLower();
            List<IRegistration> rtn = new List<IRegistration>();
            //ask each service to get pending registrations
            var pedQuery = _dogRegService.SearchPedigreeRegistrationsByDogName(searchText);
            var pedigrees = await pedQuery.ToListAsync();
            ICollection<LitterRegistrationModel> litters = await _litterService.SearchRegistrationsByDogName(searchText);
            ICollection<PuppyRegistrationModel> puppies = await _dogRegService.SearchPuppyRegistrationsByDogName(searchText);
            ICollection<JuniorHandlerRegistrationModel> handlers = await _jrHandlerService.SearchRegistrationsByDogName(searchText);
            rtn.AddRange(pedigrees);
            rtn.AddRange(litters);
            rtn.AddRange(puppies);
            rtn.AddRange(handlers);
            return sortRegistrations(rtn);
        }

        #region Payment

        /// <summary>
        /// finds the cost per registration, subtotal, and any processing fees
        /// These are user dependant
        /// </summary>
        /// <param name="registrationsToSubmit"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<PaymentQuoteDTO> GenerateSubmitQuote(ICollection<RegistrationSubmitDTO> registrationsToSubmit, UserModel user)
        {
            PaymentQuoteDTO quote = new PaymentQuoteDTO
            {

            };
            bool isRepresentative = false;

            if (user.Roles.FirstOrDefault(r => r.Type == SystemRoleEnum.Representative) != null)
            {
                //representative, no processing fee, custom registration fees
                isRepresentative = true;
            }

            FeesModel fees = await getFees(user, isRepresentative);
            if (!isRepresentative)
            {
                quote.TransactionFee = fees.OnlineFee;
            }

            foreach (RegistrationSubmitDTO reg in registrationsToSubmit)
            {
                try
                {
                    IRegistration registration = null;//await getRegistration(reg.RegistrationId, reg.RegistrationType);
                    PaymentItemDTO entry = null; ;
                    switch (reg.RegistrationType)
                    {
                        case RegistrationTypeEnum.Pedigree:
                            registration = await _dogRegService.CanSubmitPedigreeRegistration(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.Pedigree,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        case RegistrationTypeEnum.Puppy:
                            registration = await _dogRegService.CanSubmitPuppyRegistration(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.Puppy,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        case RegistrationTypeEnum.Transfer:
                            registration = await _dogRegService.CanSubmitPuppyRegistration(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.Transfer,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        case RegistrationTypeEnum.Litter:
                            registration = await _litterService.CanSubmitRegistration(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.Litter,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        case RegistrationTypeEnum.JuniorHandler:
                            registration = await _jrHandlerService.CanSubmitRegistration(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.JrHandler,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        case RegistrationTypeEnum.BullyId:
                            //registration = await _dogRegService.CanSubmitRegistration(reg.RegistrationId);
                            registration = await _dogRegService.GetBullyIdRequestById(reg.RegistrationId);
                            entry = new PaymentItemDTO
                            {
                                Amount = fees.BullyID,
                                RegistrationId = reg.RegistrationId,
                                RegistrationType = reg.RegistrationType,
                            };
                            break;
                        default:
                            break;
                    }
                    if (registration != null && entry != null)
                    {
                        if (registration.IsInternationalRegistration)
                        {
                            entry.Amount += fees.InternationalCharge;
                        }
                        else if (registration.OvernightRequested)
                        {
                            entry.Amount += fees.OvernightCharge;
                        }
                        else if (registration.RushRequested)
                        {
                            entry.Amount += fees.RushCharge;
                        }
                        quote.Registrations.Add(entry);
                    }
                    else
                    {
                        quote.InvalidRegistrations.Add(new InvalidItemDTO
                        {
                            Amount = 0,
                            RegistrationId = reg.RegistrationId,
                            RegistrationType = reg.RegistrationType,
                            Reason = $"Registration for Id {reg.RegistrationId} not found in the system"
                        });
                    }
                }
                catch (Exception e)
                {
                    quote.InvalidRegistrations.Add(new InvalidItemDTO
                    {
                        Amount = 0,
                        RegistrationId = reg.RegistrationId,
                        RegistrationType = reg.RegistrationType,
                        Reason = e.Message
                    });
                }
            }
            quote.SubTotal = quote.Registrations.Any() ? quote.Registrations.Sum(r => r.Amount) : 0;
            return quote;
        }

        private async Task<FeesModel> getFees(UserModel user, bool isRepresentative)
        {
            FeesModel fees = _registrationFees;
            //if rep, grab the rep meta data
            if (isRepresentative)
            {
                RepresentativeModel rep = await _context.Representatives.FirstOrDefaultAsync(r => r.UserRecord != null && r.UserRecord.Id == user.Id);
                if (rep.PedigreeRegistrationFee != 0)
                {
                    fees.Pedigree = rep.PedigreeRegistrationFee;
                }
                if (rep.PuppyRegistrationFee != 0)
                {
                    fees.Puppy = rep.PuppyRegistrationFee;
                }
                if (rep.TransferFee != 0)
                {
                    fees.Transfer = rep.TransferFee;
                }
                if (rep.LitterRegistrationFee != 0)
                {
                    fees.Litter = rep.LitterRegistrationFee;
                }
                if (rep.JrHandlerRegistrationFee != 0)
                {
                    fees.JrHandler = rep.JrHandlerRegistrationFee;
                }
                if (rep.BullyIdRequestFee != 0)
                {
                    fees.BullyID = rep.BullyIdRequestFee;
                }
            }

            return fees;
        }

        #endregion


        private ICollection<IRegistration> sortRegistrations(ICollection<IRegistration> regs)
        {
            regs = regs.OrderBy(r => r.DateSubmitted).ToList();
            var rtnList = new List<IRegistration>();
            if (regs == null || !regs.Any())
            {
                return rtnList;
            }
            var overnight = regs.Where(r => r.OvernightRequested);
            regs = regs.Except(overnight).ToList();
            rtnList.AddRange(overnight);

            var rush = regs.Where(r => r.RushRequested);
            regs = regs.Except(rush).ToList();
            rtnList.AddRange(rush);

            rtnList.AddRange(regs.Where(r => r.IsInternationalRegistration));
            rtnList.AddRange(regs.Except(rtnList));//get the rest
            return rtnList;
        }
        public async Task<IRegistration> GetRegistration(int registrationId, RegistrationTypeEnum registrationType) => await getRegistration(registrationId, registrationType);

        private async Task<IRegistration> getRegistration(int registrationId, RegistrationTypeEnum registrationType)
        {
            switch (registrationType)
            {
                case RegistrationTypeEnum.Pedigree:
                    return await _dogRegService.GetPedigreeRegistrationByIdAsync(registrationId);
                //return await _context.Registrations.Where(r => r.Id == registrationId).FirstOrDefaultAsync();
                case RegistrationTypeEnum.Puppy:
                case RegistrationTypeEnum.Transfer:
                    return await _dogRegService.GetPuppyRegistrationByIdAsync(registrationId);
                // return await _context.PuppyRegistrations.Where(r => r.Id == registrationId).FirstOrDefaultAsync();
                case RegistrationTypeEnum.Litter:
                    return await _litterService.GetLitterRegistrationById(registrationId);
                case RegistrationTypeEnum.JuniorHandler:
                    return await _jrHandlerService.GetJrRegistrationById(registrationId);
                case RegistrationTypeEnum.BullyId:
                    return await _dogRegService.GetBullyIdRequestById(registrationId);

                default:
                    return null;
            }
        }


        private string GetThumbnailBase64String(AttachmentModel attachment)
        {
            using (Image<Rgba32> image = Image.Load(attachment.Data))
            {
                image.Mutate(x => x
                     .Resize(48, 48));
                var format = image.GetConfiguration()
                    .ImageFormatsManager
                    .FindFormatByFileExtension(System.IO.Path.GetExtension(attachment.FileName));
                return image.ToBase64String(format);
            }
        }
    }
}