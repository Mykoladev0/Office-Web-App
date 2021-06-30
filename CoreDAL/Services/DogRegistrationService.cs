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

namespace CoreDAL.Services
{
    public class DogRegistrationService : IDogRegistrationService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IMapper _autoMapper;
        private readonly IOwnerService _ownerService;
        private readonly IDogService _dogService;
        private readonly IRegistrationNotificationService _notificationService;
        public DogRegistrationService(ABKCOnlineContext context,
        IOwnerService ownerService, IDogService dogService,
        IMapper autoMapper, IRegistrationNotificationService notificationService)
        {
            _context = context;
            _autoMapper = autoMapper;
            _ownerService = ownerService;
            _dogService = dogService;
            _notificationService = notificationService;
        }

        public async Task<ICollection<RegistrationModel>> GetAllPedigreeRegistrations()
        {
            return await _context.Registrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles).Include(r => r.StatusHistory).ToListAsync();
        }

        public async Task<RegistrationModel> GetPedigreeRegistrationByIdAsync(int id)
        {
            RegistrationModel found = await _context.Registrations
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Sire)
            .Include(r => r.DogInfo.Dam)
            .Include(r => r.AssociatedTransaction)
            .Include(r => r.StatusHistory).Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Owner).Include(r => r.DogInfo.CoOwner)
                .FirstOrDefaultAsync(r => r.Id == id);
            return found;
        }

        public async Task<IRegistration> CanSubmitPedigreeRegistration(int registrationId)
        {
            // _context.ChangeTracker.LazyLoadingEnabled = true;
            RegistrationModel found = await _context.Registrations
                .Include(r => r.StatusHistory)
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.DogInfo.Breed)
                .FirstOrDefaultAsync(r => r.Id == registrationId);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} could not be found");
            }
            if (found.CurrentStatus.Status == RegistrationStatusEnum.Approved)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} has already been approved.");
            }
            //make sure all the pieces are here
            ICollection<SupportingDocumentTypeEnum> supportingDocs = GetPedigreeDocsProvided(registrationId);
            if (!supportingDocs.Contains(SupportingDocumentTypeEnum.FrontPhoto))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have a front photo.");
            }
            if (!supportingDocs.Contains(SupportingDocumentTypeEnum.SidePhoto))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have a side photo.");
            }
            if (!supportingDocs.Contains(SupportingDocumentTypeEnum.FrontPedigree))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have a front pedigree attached.");
            }
            if (!supportingDocs.Contains(SupportingDocumentTypeEnum.OwnerSignature))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have an owner signature attached.");
            }
            if (found.DogInfo == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have associated dog information.");
            }
            if (found.DogInfo.Owner == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have owner information.");
            }
            if (found.DogInfo.CoOwner != null && !supportingDocs.Contains(SupportingDocumentTypeEnum.CoOwnerSignature))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} has a co-owner, but no co-owner signature on record.");
            }
            if (found.DogInfo.DateOfBirth == DateTime.MinValue)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not a date of birth.");
            }
            if (found.DogInfo.Breed == null)
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have breed selected.");
            }
            if (string.IsNullOrEmpty(found.DogInfo.DogName))
            {
                throw new InvalidOperationException($"Registration for id {registrationId} does not have dog name provided.");
            }
            return found;
        }

        public async Task<RegistrationModel> StartDraftPedigreeRegistration(BaseDogDTO dogInfo, UserModel savedBy)
        {
            //new one!
            if (dogInfo.Id > 0)
            {
                throw new InvalidOperationException($"To Start a Draft Pedigree Registration, it must be with a new dog not in the ABKC system");
            }
            BaseDogModel dog = _autoMapper.Map<BaseDogModel>(dogInfo);
            dog = await _dogService.UpdateFromDTO(dog, dogInfo, false);
            dog.Id = 0;
            dog.Owner = null;
            dog.CoOwner = null;
            dog.LastModifiedBy = savedBy;
            await _context.ABKCDogs.AddAsync(dog);
            RegistrationModel toSave = new RegistrationModel()
            {
                DogInfo = dog
            };
            //we are back in draft status!
            if (toSave.CurrentStatus == null || toSave.CurrentStatus.Status != RegistrationStatusEnum.Draft)
            {
                toSave.StatusHistory.Add(new DogRegistrationStatusModel()
                {
                    Status = RegistrationStatusEnum.Draft,
                    StatusChangedBy = savedBy//getuser,

                });
            }
            _context.Registrations.Add(toSave);
            // toSave.SubmittedBy = savedBy;
            await _context.SaveChangesAsync();
            return toSave;
        }

        public async Task<RegistrationModel> SaveDraftPedigreeRegistration(PedigreeRegistrationDraftDTO registration, UserModel savedBy)
        {
            //check to see if it has a valid id, makes it an update
            RegistrationModel toSave = null;
            //new registration
            if (registration.Id < 1)
            {
                throw new InvalidOperationException($"To Save Draft Pedigree Registration, it first must be started");
            }
            //existing, need logic :)
            toSave = await _context.Registrations
                .Include(r => r.DogInfo)
                .Include(r => r.StatusHistory)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.DogInfo.Sire)
                .Include(r => r.DogInfo.Dam)
                .FirstOrDefaultAsync(r => r.Id == registration.Id);
            if (toSave == null)
            {
                return null;
                // throw new InvalidOperationException($"No registration with Id {registration.Id} could be found");
            }
            //update dog details
            if (registration.DogInfo != null)
            {
                //do line by line changes or just apply them?
                BaseDogModel dog = _autoMapper.Map<BaseDogModel>(registration.DogInfo);
                if (dog.Id > 0)
                {
                    registration.DogInfo.OwnerId = 0;//clear this out because the registration will set it!
                    registration.DogInfo.CoOwnerId = 0;//clear this out because the registration will set it!
                    BaseDogModel found = await _context.ABKCDogs.FirstOrDefaultAsync(d => d.Id == dog.Id);
                    found = await _dogService.UpdateFromDTO(found, registration.DogInfo, false);
                    // _context.Entry(found).CurrentValues.SetValues(dog);
                    toSave.DogInfo = found;
                }
                else
                {
                    toSave.DogInfo = dog;
                }

            }
            //update owner details
            if (registration.Owner != null)
            {
                // if (toSave.DogInfo == null)
                // {
                //     toSave.DogInfo = new BaseDogModel()
                //     {
                //         DogName = ""//do I use a placeholder name here?
                //     };
                // }
                Owners owner = _autoMapper.Map<Owners>(registration.Owner);
                if (owner.Id > 0)
                {
                    Owners found = await _context.Owners.FirstOrDefaultAsync(d => d.Id == owner.Id);
                    found = await _ownerService.UpdateFromDTO(found, registration.Owner, false);
                    toSave.DogInfo.Owner = found;
                }
                else
                {
                    owner.Id = 0;
                    owner = await _ownerService.AddOwner(owner, savedBy);
                    toSave.DogInfo.Owner = owner;
                }
            }
            //update co-owner details
            if (registration.CoOwner != null)
            {
                // if (toSave.DogInfo == null)
                // {
                //     toSave.DogInfo = new BaseDogModel()
                //     {
                //         DogName = ""//do I use a placeholder name here?
                //     };
                // }
                Owners coOwner = _autoMapper.Map<Owners>(registration.CoOwner);
                if (coOwner.Id > 0)
                {
                    Owners found = await _context.Owners.FirstOrDefaultAsync(d => d.Id == coOwner.Id);
                    found = await _ownerService.UpdateFromDTO(found, registration.Owner, false);
                    toSave.DogInfo.CoOwner = found;
                }
                else
                {
                    coOwner.Id = 0;
                    coOwner = await _ownerService.AddOwner(coOwner, savedBy);
                    toSave.DogInfo.CoOwner = coOwner;
                }
            }

            toSave.IsInternationalRegistration = registration.IsInternational;
            toSave.OvernightRequested = registration.OvernightRequested.HasValue ? registration.OvernightRequested.Value : toSave.OvernightRequested;
            toSave.RushRequested = registration.RushRequested.HasValue ? registration.RushRequested.Value : toSave.RushRequested;

            //we are back in draft status!
            if (toSave.CurrentStatus == null || toSave.CurrentStatus.Status != RegistrationStatusEnum.Draft)
            {
                toSave.SetStatus(RegistrationStatusEnum.Draft, savedBy);
            }
            // toSave.SubmittedBy = savedBy;
            await _context.SaveChangesAsync();
            return toSave;
        }

        public async Task<ICollection<RegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel user)
        {
            var q = _context.Registrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles).AsQueryable();
            if (status != null)
            {
                q = q.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == status.Value);
            }

            if (user != null)
            {
                q = q.Where(r => r.StatusHistory.OrderBy(s => s.DateModified)
                    .Where(s => s.StatusChangedBy != null).FirstOrDefault().StatusChangedBy.Id == user.Id);
                // q = q.Where(r => r.SubmittedBy != null && r.SubmittedBy.Id == user.Id);
            }
            var l = await q.OrderBy(r => r.DateModified).ToListAsync();
            return l ?? new List<RegistrationModel>();

        }


        public async Task<IRegistration> ApprovePedigreeRegistration(RegistrationModel reg, UserModel user)
        {
            //need to add dog to system

            //what do we do with the dog, if anything in the DB?
            //need to create an entry in the ABKC Dogs table which will just hold and FK and the ABKC #?
            //get abkc #
            //near-term, add it to old table
            Dogs origDb = _autoMapper.Map<Dogs>(reg.DogInfo);
            Dogs sire = await _dogService.GetById(reg.DogInfo.Sire != null ? reg.DogInfo.Sire.OriginalDogTableId : -1);
            origDb.SireNo = sire?.Id;
            Dogs dam = await _dogService.GetById(reg.DogInfo.Dam != null ? reg.DogInfo.Dam.OriginalDogTableId : -1);
            origDb.DamNo = dam?.Id;
            origDb.Registered = true;
            origDb.DateRegistered = DateTime.UtcNow;
            origDb.ModifiedBy = user.LoginName;

            //following method will generate an ABKC Number and BullyId for the dog
            int oldId = await _dogService.AddDog(origDb);
            reg.DogInfo.OriginalDogTableId = oldId;
            return reg;
        }

        public IQueryable<RegistrationModel> GetPedigreeRegistrationsByRepresentativeQuery(int repId)
        {
            return _context.Registrations.Include(reg => reg.StatusHistory).Where(reg => reg.SubmittedBy != null && reg.SubmittedBy.Id == repId);
        }
        public IQueryable<RegistrationModel> GetPedigreeRegistrationsByOwnerQuery(int ownerId)
        {
            return _context.Registrations
              .Where(reg => reg.DogInfo != null && (reg.DogInfo.Owner != null &&
                reg.DogInfo.Owner.Id == ownerId) ||
                (reg.DogInfo.CoOwner != null && reg.DogInfo.CoOwner.Id == ownerId));
        }

        public IQueryable<RegistrationModel> SearchPedigreeRegistrationsByRepresentativeQuery(string searchText)
        {
            return _context.Registrations.Where(reg => reg.SubmittedBy != null
                && reg.SubmittedBy.LoginName.ToLower().Contains(searchText.ToLower()));
        }
        public IQueryable<RegistrationModel> SearchPedigreeRegistrationsByOwner(string searchText)
        {
            string searchLower = searchText.ToLower();
            return _context.Registrations
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Where(reg => reg.DogInfo != null && (reg.DogInfo.Owner != null &&
                reg.DogInfo.Owner.LastName.ToLower().Contains(searchLower)) ||
                (reg.DogInfo.CoOwner != null && reg.DogInfo.CoOwner.LastName.ToLower().Contains(searchLower)));
        }
        public IQueryable<RegistrationModel> SearchPedigreeRegistrationsByDogName(string searchText)
        {
            string searchLower = searchText.ToLower();
            return _context.Registrations
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Where(reg => reg.DogInfo != null && (reg.DogInfo.DogName.ToLower().Contains(searchLower)));
        }

        public ICollection<SupportingDocumentTypeEnum> GetPedigreeDocsProvided(int id)
        {
            List<SupportingDocumentTypeEnum> rtn = new List<SupportingDocumentTypeEnum>();
            var found = _context.Registrations.Where(r => r.Id == id && r.FrontPhoto != null).Select(r => r.FrontPhoto.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.FrontPhoto);
            }
            found = _context.Registrations.Where(r => r.Id == id && r.SidePhoto != null).Select(r => r.SidePhoto.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.SidePhoto);
            }
            found = _context.Registrations.Where(r => r.Id == id && r.FrontPedigree != null).Select(r => r.FrontPedigree.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.FrontPedigree);
            }
            found = _context.Registrations.Where(r => r.Id == id && r.BackPedigree != null).Select(r => r.BackPedigree.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.BackPedigree);
            }
            found = _context.Registrations.Where(r => r.Id == id && r.OwnerSignature != null).Select(r => r.OwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.OwnerSignature);
            }
            found = _context.Registrations.Where(r => r.Id == id && r.CoOwnerSignature != null).Select(r => r.CoOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.CoOwnerSignature);
            }
            return rtn;
        }

        public async Task<AttachmentModel> GetSupportingPedigreeDocument(int id, SupportingDocumentTypeEnum documentType)
        {
            IQueryable<RegistrationModel> q = _context.Registrations.Where(r => r.Id == id);

            if (!q.Any())
            {
                return null;
            }
            RegistrationModel reg = null;
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.FrontPhoto:
                    reg = await q.Include(r => r.FrontPhoto).FirstOrDefaultAsync();
                    return reg.FrontPhoto;
                case SupportingDocumentTypeEnum.SidePhoto:
                    reg = await q.Include(r => r.SidePhoto).FirstOrDefaultAsync();
                    return reg.SidePhoto;
                case SupportingDocumentTypeEnum.FrontPedigree:
                    reg = await q.Include(r => r.FrontPedigree).FirstOrDefaultAsync();
                    return reg.FrontPedigree;
                case SupportingDocumentTypeEnum.BackPedigree:
                    reg = await q.Include(r => r.BackPedigree).FirstOrDefaultAsync();
                    return reg.BackPedigree;

                case SupportingDocumentTypeEnum.OwnerSignature:
                    reg = await q.Include(r => r.OwnerSignature).FirstOrDefaultAsync();
                    return reg.OwnerSignature;
                case SupportingDocumentTypeEnum.CoOwnerSignature:
                    reg = await q.Include(r => r.CoOwnerSignature).FirstOrDefaultAsync();
                    return reg.CoOwnerSignature;

            }
            return null;
        }

        public async Task<bool> DeletePedigreeRegistration(int id)
        {
            RegistrationModel reg = await _context.Registrations.Include(r => r.StatusHistory).FirstOrDefaultAsync(r => r.Id == id);
            if (reg != null)
            {
                if (reg.CurStatus == RegistrationStatusEnum.Approved)
                {
                    throw new InvalidOperationException("Approved registrations cannot be deleted");
                }
                _context.RemoveRange(reg.StatusHistory);
                _context.Registrations.Remove(reg);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #region Puppy registration

        /// <summary>
        /// retrieves the dog by id and starts a puppy registration if that dog is eligable
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        public async Task<PuppyRegistrationModel> StartPuppyRegistration(int dogId, UserModel savedBy, bool isTransferRequest = false)
        {
            //get dog from abkc table, adding to abkc table if it only exists in original table
            BaseDogModel abkcDog = await _dogService.GetDogByOldTableId(dogId);
            if (abkcDog == null)
            {
                throw new InvalidOperationException($"Dog with id {dogId} could not be found");
            }
            //See if dog is eligible?
            if (!isTransferRequest)
            {
                if (!abkcDog.DogName.ToLowerInvariant().Contains("unregistered puppy"))
                {
                    throw new InvalidOperationException($"Dog being registered as a puppy has already been registered with the name {abkcDog.DogName} for id {dogId}");
                }
            }
            PuppyRegistrationModel pReg = new PuppyRegistrationModel
            {
                DogInfo = abkcDog,
                IsTransferRequest = isTransferRequest
            };
            if (pReg.CurrentStatus == null || pReg.CurrentStatus.Status != RegistrationStatusEnum.Draft)
            {
                pReg.StatusHistory.Add(new PuppyRegistrationStatusModel()
                {
                    Status = RegistrationStatusEnum.Draft,
                    StatusChangedBy = savedBy//getuser,

                });
            }
            _context.PuppyRegistrations.Add(pReg);
            await _context.SaveChangesAsync();
            return pReg;

        }

        /// <summary>
        /// updates an existing puppy registration draft will new/changed information provided in the registration dto
        /// note: only microchip and color dog information may be updated
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="reg"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<PuppyRegistrationModel> SavePuppyDraft(int regId, PuppyRegistrationDraftDTO reg, UserModel user)
        {
            PuppyRegistrationModel found = await _context.PuppyRegistrations
                .Include(d => d.DogInfo)
                .Include(d => d.DogInfo.Color)
                .Include(d => d.NewOwner)
                .Include(d => d.NewCoOwner)
                .Include(r => r.StatusHistory)
                .Include(d => d.DogInfo.Breed)
                .Include(d => d.DogInfo.Color)
                .Include(d => d.DogInfo.Owner)
                .Include(d => d.DogInfo.CoOwner)
                .Include(d => d.DogInfo.Sire)
                .Include(d => d.DogInfo.Dam)
                .FirstOrDefaultAsync(p => p.Id == regId);
            if (found == null)
            {
                return null;
                // throw new InvalidOperationException($"No Puppy registration for {regId} could be found to update");
            }

            if (reg.SellDate.HasValue)
            {
                found.DateOfSale = reg.SellDate;
            }
            if (!found.IsTransferRequest)//transfer requests can only change sell date, and new owners
            {
                //for puppy registrations
                if (reg.ColorId > 1)
                {
                    if (found.DogInfo.Color == null || found.DogInfo.Color.Id != reg.ColorId)
                    {
                        Colors color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == reg.ColorId);
                        found.DogInfo.Color = color;
                        found.DogInfo.LastModifiedBy = user;
                    }
                }
                if (!string.IsNullOrEmpty(reg.MicrochipNumber))
                {
                    found.DogInfo.MicrochipNumber = reg.MicrochipNumber;
                    found.DogInfo.LastModifiedBy = user;
                }
                if (!string.IsNullOrEmpty(reg.DogName) && reg.DogName != found.DogInfo.DogName)
                {
                    found.DogInfo.DogName = reg.DogName;
                    found.DogInfo.LastModifiedBy = user;
                }
                if (!string.IsNullOrEmpty(reg.MicrochipType))
                {
                    //TODO: Add microchip type to dog model!
                    // found.DogInfo.MicrochipType = reg.MicrochipType;
                    //dogInfoChanged = true;
                    found.DogInfo.LastModifiedBy = user;
                }
            }

            if (reg.NewOwner != null && reg.NewOwner.Id == 0)
            {
                //new owner to add
                Owners newOwner = _autoMapper.Map<Owners>(reg.NewOwner);
                await _ownerService.AddOwner(newOwner, user);
                found.NewOwner = newOwner;
            }
            else
            {
                if (reg.NewOwner != null && reg.NewOwner.Id > 1 && (found.NewOwner == null || found.NewOwner.Id != reg.NewOwner.Id))
                {
                    Owners owner = await _ownerService.GetById(reg.NewOwner.Id);
                    found.NewOwner = owner;
                }
            }

            if (reg.NewCoOwner != null && reg.NewCoOwner.Id == 0)
            {
                //new owner to add
                Owners newCoOwner = _autoMapper.Map<Owners>(reg.NewCoOwner);
                await _ownerService.AddOwner(newCoOwner, user);
                found.NewCoOwner = newCoOwner;
            }
            else
            {
                if (reg.NewCoOwner != null && reg.NewCoOwner.Id > 1 && (found.NewCoOwner == null || found.NewCoOwner.Id != reg.NewCoOwner.Id))
                {
                    Owners owner = await _ownerService.GetById(reg.NewCoOwner.Id);
                    found.NewCoOwner = owner;
                }
            }
            found.IsInternationalRegistration = reg.IsInternational;
            found.OvernightRequested = reg.OvernightRequested.HasValue ? reg.OvernightRequested.Value : found.OvernightRequested;
            found.RushRequested = reg.RushRequested.HasValue ? reg.RushRequested.Value : found.RushRequested;

            if (found.CurStatus != RegistrationStatusEnum.Draft)
            {
                found.SetStatus(RegistrationStatusEnum.Draft, user);
            }

            await _context.SaveChangesAsync();
            return found;
        }

        /// <summary>
        /// retrieves a puppy registration by dogId searching new and old tables
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        public async Task<PuppyRegistrationModel> GetPuppyRegistrationForDogId(int dogId)
        {
            PuppyRegistrationModel found = await _context.PuppyRegistrations
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.NewCoOwner)
                .Include(r => r.NewOwner)
                .Include(r => r.SubmittedBy)
                .Include(r => r.StatusHistory)
                .FirstOrDefaultAsync(r => r.DogInfo != null && (r.DogInfo.OriginalDogTableId == dogId || r.DogInfo.Id == dogId));
            return found;
        }

        public async Task<PuppyRegistrationModel> GetPuppyRegistrationByIdAsync(int id)
        {
            PuppyRegistrationModel found = await _context.PuppyRegistrations
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.NewCoOwner)
                .Include(r => r.NewOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.AssociatedTransaction)
                .FirstOrDefaultAsync(r => r.Id == id);
            return found;
        }


        public async Task<AttachmentModel> GetSupportingPuppyDocument(int id, SupportingDocumentTypeEnum documentType)
        {
            IQueryable<PuppyRegistrationModel> q = _context.PuppyRegistrations.Where(r => r.Id == id);

            if (!q.Any())
            {
                return null;
            }
            PuppyRegistrationModel reg = null;
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.SellerSignature:
                    reg = await q.Include(r => r.SellerSignature).FirstOrDefaultAsync();
                    return reg.SellerSignature;
                case SupportingDocumentTypeEnum.CoSellerSignature:
                    reg = await q.Include(r => r.CoSellerSignature).FirstOrDefaultAsync();
                    return reg.CoSellerSignature;
                case SupportingDocumentTypeEnum.OwnerSignature:
                    reg = await q.Include(r => r.OwnerSignature).FirstOrDefaultAsync();
                    return reg.OwnerSignature;
                case SupportingDocumentTypeEnum.CoOwnerSignature:
                    reg = await q.Include(r => r.CoOwnerSignature).FirstOrDefaultAsync();
                    return reg.CoOwnerSignature;
                case SupportingDocumentTypeEnum.BillOfSaleFront:
                    reg = await q.Include(r => r.BillOfSaleFront).FirstOrDefaultAsync();
                    return reg.BillOfSaleFront;
                case SupportingDocumentTypeEnum.BillOfSaleBack:
                    reg = await q.Include(r => r.BillOfSaleBack).FirstOrDefaultAsync();
                    return reg.BillOfSaleBack;
            }
            return null;
        }
        public ICollection<SupportingDocumentTypeEnum> GetPuppyDocsProvided(int id)
        {
            List<SupportingDocumentTypeEnum> rtn = new List<SupportingDocumentTypeEnum>();
            var found = _context.PuppyRegistrations.Where(r => r.Id == id && r.SellerSignature != null).Select(r => r.SellerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.SellerSignature);
            }
            found = _context.PuppyRegistrations.Where(r => r.Id == id && r.CoSellerSignature != null).Select(r => r.CoSellerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.CoSellerSignature);
            }
            found = _context.PuppyRegistrations.Where(r => r.Id == id && r.NewOwner != null).Select(r => r.NewOwner.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.OwnerSignature);
            }
            found = _context.PuppyRegistrations.Where(r => r.Id == id && r.CoOwnerSignature != null).Select(r => r.CoOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.CoOwnerSignature);
            }
            found = _context.PuppyRegistrations.Where(r => r.Id == id && r.BillOfSaleFront != null).Select(r => r.BillOfSaleFront.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.BillOfSaleFront);
            }
            found = _context.PuppyRegistrations.Where(r => r.Id == id && r.BillOfSaleBack != null).Select(r => r.BillOfSaleBack.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.BillOfSaleBack);
            }
            return rtn;
        }

        public async Task<bool> DeletePuppyRegistration(int id)
        {
            PuppyRegistrationModel reg = await _context.PuppyRegistrations.Include(r => r.StatusHistory).FirstOrDefaultAsync(r => r.Id == id);
            if (reg != null)
            {
                if (reg.CurStatus == RegistrationStatusEnum.Approved)
                {
                    throw new InvalidOperationException("Approved registrations cannot be deleted");
                }
                _context.RemoveRange(reg.StatusHistory);
                _context.PuppyRegistrations.Remove(reg);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IRegistration> CanSubmitPuppyRegistration(int regId)
        {
            PuppyRegistrationModel found = await _context.PuppyRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.DogInfo.Breed)
                .FirstOrDefaultAsync(r => r.Id == regId);
            if (found == null)
            {
                throw new InvalidOperationException($"Registration for id {regId} could not be found");
            }
            if (found.CurrentStatus.Status == RegistrationStatusEnum.Approved)
            {
                throw new InvalidOperationException($"Registration for id {regId} has already been approved.");
            }
            //make sure all the pieces are here
            ICollection<SupportingDocumentTypeEnum> supportingDocs = GetPuppyDocsProvided(regId);
            if (!found.IsTransferRequest)
            {
                //puppy registration
                if (!supportingDocs.Contains(SupportingDocumentTypeEnum.SellerSignature))
                {
                    throw new InvalidOperationException($"Puppy Registration  for id {regId} does not have a sellers signature.");
                }
                if (!supportingDocs.Contains(SupportingDocumentTypeEnum.OwnerSignature))
                {
                    throw new InvalidOperationException($"Puppy Registration  for id {regId} does not have a new owner's signature.");
                }
                if (found.DogInfo == null)
                {
                    throw new InvalidOperationException($"Puppy Registration for id {regId} does not have associated dog information.");
                }
                if (found.DogInfo.Owner == null)
                {
                    throw new InvalidOperationException($"Puppy Registration for id {regId} does not have owner information.");
                }
                if (found.DogInfo.CoOwner != null && !supportingDocs.Contains(SupportingDocumentTypeEnum.CoSellerSignature))
                {
                    throw new InvalidOperationException($"Puppy Registration for id {regId} has a co-owner, but no co-owner seller signature on record.");
                }
                if (found.NewCoOwner != null && !supportingDocs.Contains(SupportingDocumentTypeEnum.CoOwnerSignature))
                {
                    throw new InvalidOperationException($"Puppy Registration for id {regId} has a co-owner, but no co-owner signature on record.");
                }
                if (found.DogInfo.Color == null)
                {
                    throw new InvalidOperationException($"Registration for id {regId} does not have color selected.");
                }
                if (string.IsNullOrEmpty(found.DogInfo.DogName))
                {
                    throw new InvalidOperationException($"Registration for id {regId} does not have dog name provided.");
                }
            }
            else
            {
                //transfer registration
                //check transfer papers
                if (!supportingDocs.Contains(SupportingDocumentTypeEnum.BillOfSaleFront))
                {
                    throw new InvalidOperationException($"Registration for id {regId} does not have a front bill of sale included.");
                }
                if (!supportingDocs.Contains(SupportingDocumentTypeEnum.BillOfSaleBack))
                {
                    throw new InvalidOperationException($"Registration for id {regId} does not have a back bill of sale included.");
                }
            }
            if (found.DateOfSale == null || found.DateOfSale == DateTime.MinValue)
            {
                throw new InvalidOperationException($"Registration for id {regId} does not a date of sale.");
            }

            return found;
        }

        public async Task<IRegistration> ApprovePuppyRegistration(PuppyRegistrationModel reg, UserModel user)
        {

            Dogs origDog = _autoMapper.Map<Dogs>(reg.DogInfo);


            //is this a transfer?
            Transfers newTransfer = new Transfers
            {
                DogId = origDog.Id,
                OldOwnerId = origDog.OwnerId,
                OldCoOwnerId = origDog.CoOwnerId,
                SaleDate = reg.DateOfSale,
                NewOwnerId = reg.NewOwner.Id,
                NewCoOwnerId = reg.NewCoOwner?.Id,
                ModifiedBy = user.LoginName,
                ModifyDate = DateTime.UtcNow,
            };
            _context.Transfers.Add(newTransfer);
            reg.TransferCreatedFromRegistration = newTransfer;

            origDog.OwnerId = reg.NewOwner.Id;
            origDog.CoOwnerId = reg.NewCoOwner?.Id;
            origDog.DogName = reg.DogInfo.DogName;
            origDog.Color = reg.DogInfo.Color.Color;
            origDog.ChipNo = reg.DogInfo.MicrochipNumber;

            origDog.Registered = true;
            origDog.DateRegistered = DateTime.UtcNow;
            origDog.ModifiedBy = user.LoginName;

            int oldId = await _dogService.UpdateDog(origDog);
            reg.DogInfo.OriginalDogTableId = oldId;
            return reg;
        }

        public async Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByRepresentative(int repId)
        {
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
            .Where(reg => reg.SubmittedBy != null && reg.SubmittedBy.Id == repId).ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByOwner(int ownerId)
        {
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
            .Where(r => r.DogInfo != null && r.DogInfo != null && r.DogInfo.Owner.Id == ownerId).ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByRepresentative(string searchText)
        {
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.SubmittedBy != null
                && reg.SubmittedBy.LoginName.ToLower().Contains(searchText.ToLower())).ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByOwner(string searchText)
        {
            searchText = searchText.ToLower();
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.DogInfo != null && (reg.DogInfo.Owner != null &&
                reg.DogInfo.Owner.LastName.ToLower().Contains(searchText)) ||
                (reg.DogInfo.CoOwner != null && reg.DogInfo.CoOwner.LastName.ToLower().Contains(searchText))).ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByDogName(string searchText)
        {
            searchText = searchText.ToLower();
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
            .Where(reg => reg.DogInfo != null && (reg.DogInfo.DogName.ToLower().Contains(searchText))).ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null)
        {
            var q = _context.PuppyRegistrations
            .Include(r => r.DogInfo)
            .Include(r => r.DogInfo.Breed)
            .Include(r => r.DogInfo.Color)
            .Include(r => r.DogInfo.Owner)
            .Include(r => r.DogInfo.CoOwner)
            .Include(r => r.NewOwner)
            .Include(r => r.NewCoOwner)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles).AsQueryable();
            if (status != null)
            {
                q = q.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == status.Value);
            }

            if (filterByUser != null)
            {
                // q = q.Where(r => r.SubmittedBy != null && r.SubmittedBy.Id == filterByUser.Id);
                q = q.Where(r => r.StatusHistory.OrderBy(s => s.DateModified)
    .Where(s => s.StatusChangedBy != null).FirstOrDefault().StatusChangedBy.Id == filterByUser.Id);
            }
            var l = await q.OrderBy(r => r.DateModified).ToListAsync();
            return l ?? new List<PuppyRegistrationModel>();
        }

        public async Task<ICollection<PuppyRegistrationModel>> GetAllPuppyRegistrations()
        {
            ICollection<PuppyRegistrationModel> lst = await _context.PuppyRegistrations
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.NewOwner)
                .Include(r => r.NewCoOwner)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified)
                .ToListAsync();
            return lst ?? new List<PuppyRegistrationModel>();
        }

        #endregion

        #region "Bully ID Requests"

        public async Task<BullyIdRequestModel> GetBullyIdRequestForDogId(int dogId)
        {
            BullyIdRequestModel found = await _context.BullyIdRequests
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.SubmittedBy)
                .Include(r => r.StatusHistory)
                .Where(r => r.DogInfo != null && r.DogInfo.OriginalDogTableId == dogId)
                .FirstOrDefaultAsync();
            return found;
        }
        public async Task<BullyIdRequestModel> CreateBullyIdRequest(int dogId, UserModel user)
        {
            //get dog from abkc table, adding to abkc table if it only exists in original table
            BaseDogModel abkcDog = await _dogService.GetDogByOldTableId(dogId);
            if (abkcDog == null)
            {
                throw new InvalidOperationException($"Dog with id {dogId} could not be found");
            }
            BullyIdRequestModel pReg = new BullyIdRequestModel
            {
                DogInfo = abkcDog,
            };
            if (pReg.CurrentStatus == null || pReg.CurrentStatus.Status != RegistrationStatusEnum.Draft)
            {
                pReg.SetStatus(RegistrationStatusEnum.Draft, user);
            }
            _context.BullyIdRequests.Add(pReg);
            await _context.SaveChangesAsync();
            return pReg;
        }
        public async Task<bool> DeleteBullyIdRequest(int id)
        {
            BullyIdRequestModel reg = await _context.BullyIdRequests.Include(r => r.StatusHistory).FirstOrDefaultAsync(r => r.Id == id);
            if (reg != null)
            {
                if (reg.CurStatus == RegistrationStatusEnum.Approved)
                {
                    throw new InvalidOperationException("Approved registrations cannot be deleted");
                }
                _context.RemoveRange(reg.StatusHistory);
                _context.BullyIdRequests.Remove(reg);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<BullyIdRequestModel> GetBullyIdRequestById(int id)
        {
            BullyIdRequestModel found = await _context.BullyIdRequests
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.SubmittedBy)
                .Include(r => r.StatusHistory)
                .Include(r => r.AssociatedTransaction)
                .FirstOrDefaultAsync(r => r.Id == id);
            return found;
        }

        public async Task<ICollection<BullyIdRequestModel>> GetAllBullyIdRequests()
        {
            var reg = await _context.BullyIdRequests
                 .Include(r => r.DogInfo)
                 .Include(r => r.DogInfo.Breed)
                 .Include(r => r.DogInfo.Color)
                 .Include(r => r.DogInfo.Owner)
                 .Include(r => r.DogInfo.CoOwner)
                 .Include(r => r.SubmittedBy)
                 .Include(r => r.StatusHistory).ToListAsync();
            return reg ?? new List<BullyIdRequestModel>();
        }

        public async Task<ICollection<BullyIdRequestModel>> GetBullyIdRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null)
        {
            var q = _context.BullyIdRequests
                .Include(r => r.DogInfo)
                .Include(r => r.DogInfo.Breed)
                .Include(r => r.DogInfo.Color)
                .Include(r => r.DogInfo.Owner)
                .Include(r => r.DogInfo.CoOwner)
                .Include(r => r.SubmittedBy)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy.Roles).AsQueryable();
            if (status != null)
            {
                q = q.Where(r => r.StatusHistory.OrderByDescending(s => s.DateModified).FirstOrDefault().Status == status.Value);
            }

            if (filterByUser != null)
            {
                q = q.Where(r => r.StatusHistory.OrderBy(s => s.DateModified)
                    .Where(s => s.StatusChangedBy != null).FirstOrDefault().StatusChangedBy.Id == filterByUser.Id);
                // q = q.Where(r => r.SubmittedBy != null && r.SubmittedBy.Id == user.Id);
            }
            var l = await q.OrderBy(r => r.DateModified).ToListAsync();
            return l ?? new List<BullyIdRequestModel>();
        }

        public async Task<ICollection<BullyIdRequestModel>> GetBullyRequestsByRepresentative(int repId)
        {
            var reg = await _context.BullyIdRequests
               .Include(r => r.DogInfo)
               .Include(r => r.DogInfo.Breed)
               .Include(r => r.DogInfo.Color)
               .Include(r => r.DogInfo.Owner)
               .Include(r => r.DogInfo.CoOwner)
               .Include(r => r.SubmittedBy)
               .Include(r => r.StatusHistory)
               .Where(r => r.SubmittedBy != null && r.SubmittedBy.Id == repId).ToListAsync();
            return reg ?? new List<BullyIdRequestModel>();
        }

        public async Task<ICollection<BullyIdRequestModel>> GetBullyRequestsByOwner(int ownerId)
        {
            var reg = await _context.BullyIdRequests
               .Include(r => r.DogInfo)
               .Include(r => r.DogInfo.Breed)
               .Include(r => r.DogInfo.Color)
               .Include(r => r.DogInfo.Owner)
               .Include(r => r.DogInfo.CoOwner)
               .Include(r => r.SubmittedBy)
               .Include(r => r.StatusHistory)
               .Where(r => r.DogInfo != null && r.DogInfo != null && r.DogInfo.Owner.Id == ownerId)
               .ToListAsync();
            return reg ?? new List<BullyIdRequestModel>();
        }

        public async Task<IRegistration> ApproveBullyIdRequest(BullyIdRequestModel reg, UserModel user)
        {

            Dogs origDog = _autoMapper.Map<Dogs>(reg.DogInfo);
            // reg.DogInfo.OriginalDogTableId = oldId;
            //TODO: need to do something here?
            //special notification/queue about bully id requests
            return reg;
        }

        public async Task<AttachmentModel> GetSupportingBullyIdDocument(int id, SupportingDocumentTypeEnum documentType)
        {
            IQueryable<BullyIdRequestModel> q = _context.BullyIdRequests.Where(r => r.Id == id);

            if (!q.Any())
            {
                return null;
            }
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.FrontPhoto:
                    BullyIdRequestModel reg = await q.Include(r => r.FrontPhoto).FirstOrDefaultAsync();
                    return reg.FrontPhoto;
            }
            return null;
        }

        public async Task<bool> AddSupportingBullyIdRegistrationDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, UserModel savedBy)
        {
            //store documents in DB, when a registration is approved, the documents should move to azure blob storage
            BullyIdRequestModel found = await _context.BullyIdRequests.FirstOrDefaultAsync(r => r.Id == regId);
            if (found == null)
            {
                throw new InvalidOperationException($"Bully Id Request with Id {regId} could not be found, no supporting document added.");
            }
            AttachmentModel attachment = new AttachmentModel
            {
                Data = docStream,
                FileName = fileName,
            };
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.FrontPhoto:
                    found.FrontPhoto = attachment;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public ICollection<SupportingDocumentTypeEnum> GetBullyIdDocsProvided(int id)
        {
            List<SupportingDocumentTypeEnum> rtn = new List<SupportingDocumentTypeEnum>();
            //TODO:wire up bully id front photo
            // var found = _context.BullyIdRequests.Where(r => r.Id == id && r.FrontPhoto != null).Select(r => r.FrontPhoto.Id);
            // if (found != null && found.Any())
            // {
            //     rtn.Add(SupportingDocumentTypeEnum.FrontPhoto);
            // }
            return rtn;
        }


        #endregion
    }
}