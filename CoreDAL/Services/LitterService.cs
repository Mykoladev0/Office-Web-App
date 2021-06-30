using ABKCCommon.Models.DTOs.Pedigree;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class LitterService : ILitterService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IDogService _dogService;

        public LitterService(ABKCOnlineContext context, IDogService dogService)
        {
            _context = context;
            _dogService = dogService;
        }

        public async Task<int> GetLittersCount()
        {
            return await _context.Litters.CountAsync();
        }

        public async Task<int> NumberOfPups(int dogId, bool isSire)
        {
            IQueryable<Litters> foundLitters = isSire ? _context.Litters.Where(l => l.SireNo == dogId) : _context.Litters.Where(l => l.DamNo == dogId);
            int? total = await foundLitters.Select(l => l.Females + l.Males).SumAsync(s => s);
            return total ?? 0;
        }

        #region Litter Registration
        public Task<LitterRegistrationModel> GetLitterRegistrationForDog(int dogId)
        {
            throw new NotImplementedException();
        }
        public async Task<LitterRegistrationModel> SaveLitterDraft(int regId, LitterDraftDTO reg, UserModel user)
        {
            LitterRegistrationModel found = await _context.LitterRegistrations
                .Include(l => l.Breed)
                .Include(l => l.StatusHistory)
                .FirstOrDefaultAsync(p => p.Id == regId);
            if (found == null)
            {
                return null;
                // throw new InvalidOperationException($"No Litter registration for ${regId} could be found to update");
            }

            if (reg.DateOfBreeding.HasValue)
            {
                found.DateOfBreeding = reg.DateOfBreeding;
            }
            if (reg.DateOfLitterBirth.HasValue)
            {
                found.DateOfLitterBirth = reg.DateOfLitterBirth;
            }
            if (!reg.FrozenSemenUsed)
            {
                found.DateSemenCollected = null;
                found.FrozenSemenUsed = false;
            }
            else
            {
                found.FrozenSemenUsed = true;
                if (reg.DateSemenCollected.HasValue)
                {
                    found.DateSemenCollected = reg.DateSemenCollected;
                }
            }

            if (reg.Breed != null && reg.Breed.Id > 0)
            {
                found.Breed = await _context.Breeds.FirstOrDefaultAsync(b => b.Id == reg.Breed.Id);
            }
            if (reg.NumberOfFemalesBeingRegistered > 0)
            {
                found.NumberOfFemalesBeingRegistered = reg.NumberOfFemalesBeingRegistered;
            }
            if (reg.NumberOfMalesBeingRegistered > 0)
            {
                found.NumberOfMalesBeingRegistered = reg.NumberOfMalesBeingRegistered;
            }
            if (found.CurStatus != RegistrationStatusEnum.Draft)
            {
                found.SetStatus(RegistrationStatusEnum.Draft, user);
            }

            found.IsInternationalRegistration = reg.IsInternational;
            found.OvernightRequested = reg.OvernightRequested.HasValue ? reg.OvernightRequested.Value : found.OvernightRequested;
            found.RushRequested = reg.RushRequested.HasValue ? reg.RushRequested.Value : found.RushRequested;
            await _context.SaveChangesAsync();

            return found;
        }

        public async Task<LitterRegistrationModel> StartLitterRegistration(int sireId, int damId, UserModel savedBy)
        {
            //find sire by id in old table
            BaseDogModel sire = await _dogService.GetDogByOldTableId(sireId);
            if (sire == null)
            {
                throw new InvalidOperationException($"Sire with id {sireId} could not be found to start litter registration");
            }
            if (sire.Gender == BaseDogModel.GenderEnum.Female)
            {
                throw new InvalidOperationException($"Sire with id {sireId} is female in the system, contact ABKC for assistance.");
            }

            //find dam by id in old table
            BaseDogModel dam = await _dogService.GetDogByOldTableId(damId);
            if (dam == null)
            {
                throw new InvalidOperationException($"Dam with id {sireId} could not be found to start litter registration");
            }
            if (dam.Gender == BaseDogModel.GenderEnum.Male)
            {
                throw new InvalidOperationException($"Dam with id {damId} is male in the system, contact ABKC for assistance.");
            }

            LitterRegistrationModel litter = new LitterRegistrationModel
            {
                Dam = dam,
                Sire = sire,
            };
            litter.SetStatus(RegistrationStatusEnum.Draft, savedBy);
            // litter.StatusHistory.Add(new LitterRegistrationStatusModel()
            // {
            //     Status = RegistrationStatusEnum.Draft,
            //     StatusChangedBy = savedBy//getuser,
            // });
            _context.LitterRegistrations.Add(litter);
            await _context.SaveChangesAsync();
            return litter;
        }

        public async Task<Litters> CreateLitterFromRegistration(int litterRegistrationId, UserModel createdBy)
        {
            LitterRegistrationModel litterReg = await _context.LitterRegistrations
            .Include(l => l.Sire)
            .Include(l => l.Dam)
            .Include(l => l.Breed)
            .Where(l => l.Id == litterRegistrationId)
            .FirstOrDefaultAsync();
            if (litterReg == null)
            {
                throw new InvalidOperationException($"Litter with id {litterRegistrationId} could not be found");
            }
            return await createLitterFromRegistration(litterReg, createdBy);
        }

        public async Task<LitterRegistrationModel> GetLitterRegistrationById(int regId)
        {
            LitterRegistrationModel found = await _context.LitterRegistrations
            .Include(l => l.Sire)
            .Include(l => l.Sire.Owner)
            .Include(l => l.Sire.CoOwner)
            .Include(l => l.Dam)
            .Include(l => l.Dam.Owner)
            .Include(l => l.Dam.CoOwner)
            .Include(l => l.Breed)
            .Include(l => l.StatusHistory)
            .Include(r => r.AssociatedTransaction)
            .Where(l => l.Id == regId).FirstOrDefaultAsync();
            return found;
        }

        public async Task<AttachmentModel> GetSupportingDocument(int id, SupportingDocumentTypeEnum documentType)
        {
            IQueryable<LitterRegistrationModel> q = _context.LitterRegistrations.Where(r => r.Id == id);

            if (!q.Any())
            {
                return null;
            }
            LitterRegistrationModel reg = null;
            switch (documentType)
            {
                case SupportingDocumentTypeEnum.SireOwnerSignature:
                    reg = await q.Include(r => r.SireOwnerSignature).FirstOrDefaultAsync();
                    return reg.SireOwnerSignature;
                case SupportingDocumentTypeEnum.SireCoOwnerSignature:
                    reg = await q.Include(r => r.SireCoOwnerSignature).FirstOrDefaultAsync();
                    return reg.SireCoOwnerSignature;
                case SupportingDocumentTypeEnum.DamOwnerSignature:
                    reg = await q.Include(r => r.DamOwnerSignature).FirstOrDefaultAsync();
                    return reg.DamOwnerSignature;
                case SupportingDocumentTypeEnum.DamCoOwnerSignature:
                    reg = await q.Include(r => r.DamCoOwnerSignature).FirstOrDefaultAsync();
                    return reg.DamCoOwnerSignature;
            }
            return null;
        }
        public ICollection<SupportingDocumentTypeEnum> GetLitterDocsProvided(int id)
        {
            List<SupportingDocumentTypeEnum> rtn = new List<SupportingDocumentTypeEnum>();
            var found = _context.LitterRegistrations.Where(r => r.Id == id && r.SireOwnerSignature != null).Select(r => r.SireOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.SireOwnerSignature);
            }
            found = _context.LitterRegistrations.Where(r => r.Id == id && r.SireCoOwnerSignature != null).Select(r => r.SireCoOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.SireCoOwnerSignature);
            }
            found = _context.LitterRegistrations.Where(r => r.Id == id && r.DamOwnerSignature != null).Select(r => r.DamOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.DamOwnerSignature);
            }
            found = _context.LitterRegistrations.Where(r => r.Id == id && r.DamCoOwnerSignature != null).Select(r => r.DamCoOwnerSignature.Id);
            if (found != null && found.Any())
            {
                rtn.Add(SupportingDocumentTypeEnum.DamCoOwnerSignature);
            }

            return rtn;
        }

        /// <summary>
        /// verifies all required litter information is included, throws error if not
        /// will approve and then create a litter in the system from the registration
        /// this will NOT update status, that should happen from the registration service
        /// </summary>
        /// <param name="litterRegistrationId"></param>
        /// <returns></returns>
        public async Task<LitterRegistrationModel> ApproveLitterRegistration(LitterRegistrationModel litterReg, UserModel approvedBy)
        {
            litterReg = verifyInformationProvided(litterReg);
            //build a litter in the system
            await createLitterFromRegistration(litterReg, approvedBy);
            return litterReg;

        }

        public async Task<IRegistration> CanSubmitRegistration(int registrationId)
        {
            LitterRegistrationModel reg = await GetLitterRegistrationById(registrationId);
            if (reg == null)
            {
                throw new InvalidOperationException($"Cannot find registration with id {registrationId} to verify");
            }
            return verifyInformationProvided(reg);
        }


        /// <summary>
        /// returns all litter registrations that have a status of pending
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<LitterRegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null)
        {
            var q = _context.LitterRegistrations
            .Include(r => r.Sire)
            .Include(r => r.Dam)
            .Include(r => r.Dam.Owner)
            .Include(r => r.Breed)
            .Include(r => r.StatusHistory)
            .Include(r => r.SubmittedBy)
            .Include(r => r.SubmittedBy.Roles)
            .OrderBy(r => r.DateModified)
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
            return l ?? new List<LitterRegistrationModel>();
        }

        public async Task<ICollection<LitterRegistrationModel>> GetAllLitterRegistrations()
        {
            var l = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified).ToListAsync();

            return l;
        }
        #endregion

        public async Task<LitterReportDTO> BuildLitterReportFromRegistration(int id)
        {
            LitterRegistrationModel reg = await _context.LitterRegistrations
            .Include(l => l.Sire)
            .Include(l => l.Sire.Owner)
            .Include(l => l.Sire.CoOwner)
            .Include(l => l.Dam)
            .Include(l => l.Dam.Owner)
            .Include(l => l.Dam.CoOwner)
            .Include(l => l.Breed)
            .Include(l => l.LitterFromRegistration)
            .Where(l => l.Id == id).FirstOrDefaultAsync();
            if (reg == null)
            {
                throw new InvalidOperationException($"Litter Registration with Id {id} was not found");
            }
            LitterReportDTO report = new LitterReportDTO
            {
                Birthdate = reg.DateOfLitterBirth.Value,
                Breed = reg.Breed.Breed,
                SireABKCNumber = reg.Sire.ABKCNumber,
                DamABKCNumber = reg.Dam.ABKCNumber,
                SireName = reg.Sire.DogName,
                DamName = reg.Dam.DogName,
                ReportGenerationDate = DateTime.Today,
                LitterNumber = reg.LitterFromRegistration != null ? reg.LitterFromRegistration.LitterId : -1,
            };
            if (reg.LitterFromRegistration == null)
            {
                return report;
            }
            ICollection<BaseDogModel> pups = await _dogService.GetPupsFromLitterQuery(reg.LitterFromRegistration).Include(p => p.Owner).ToListAsync();
            report.PuppiesInformation = pups.Select(p => new LitterReportPuppyInformationDTO
            {
                ABKCNumber = p.ABKCNumber,
                PuppyName = p.DogName,
                Sex = p.Gender.ToString(),
                SoldTo = p.Owner?.FullName,
                City = p.Owner?.City,
                STZip = p.Owner?.State

            }).ToList();
            return report;
        }

        private LitterRegistrationModel verifyInformationProvided(LitterRegistrationModel litterReg)
        {
            ICollection<SupportingDocumentTypeEnum> docs = GetLitterDocsProvided(litterReg.Id);
            if (!docs.Contains(SupportingDocumentTypeEnum.SireOwnerSignature))
            {
                throw new InvalidOperationException($"Sire Owner signature not provided, cannot submit registration for {litterReg.Id}");
            }
            if (!docs.Contains(SupportingDocumentTypeEnum.DamOwnerSignature))
            {
                throw new InvalidOperationException($"Dam Owner signature not provided, cannot submit registration for {litterReg.Id}");
            }
            if (litterReg.Sire.CoOwner != null && !docs.Contains(SupportingDocumentTypeEnum.SireCoOwnerSignature))
            {
                throw new InvalidOperationException($"Sire Co-Owner signature not provided and the sire has a co-owner, cannot submit registration for {litterReg.Id}");
            }
            if (litterReg.Dam.CoOwner != null && !docs.Contains(SupportingDocumentTypeEnum.DamCoOwnerSignature))
            {
                throw new InvalidOperationException($"Dam Co-Owner signature not provided and the sire has a co-owner, cannot submit registration for {litterReg.Id}");
            }

            if (litterReg.NumberOfFemalesBeingRegistered == 0 && litterReg.NumberOfMalesBeingRegistered == 0)
            {
                throw new InvalidOperationException("Litters require at least one puppy to submit a registration");
            }
            if (!litterReg.DateOfLitterBirth.HasValue)
            {
                throw new InvalidOperationException("Litters require a date of birth to submit a registration");
            }
            if (litterReg.Breed == null)
            {
                throw new InvalidOperationException("Litters require a breed to submit a registration");
            }
            if (litterReg.FrozenSemenUsed && !litterReg.DateSemenCollected.HasValue)
            {
                throw new InvalidOperationException("Litters using frozen semen need a date of collection to submit a registration");
            }
            return litterReg;
        }

        private async Task<Litters> createLitterFromRegistration(LitterRegistrationModel litterReg, UserModel createdBy)
        {
            //TODO:first make sure that litter doesn't already exist

            //create new litter
            Litters litter = new Litters
            {
                Breed = litterReg.Breed.Breed,
                Birthdate = litterReg.DateOfLitterBirth,
                BreedingDate = litterReg.DateOfBreeding,
                SireNo = litterReg.Sire.OriginalDogTableId,
                DamNo = litterReg.Dam.OriginalDogTableId,
                SireOwnerId = litterReg.Sire.Owner != null ? litterReg.Sire.Owner.Id : 0,
                OwnerId = litterReg.Dam.Owner != null ? litterReg.Dam.Owner.Id : 0,//owner is Dam
                SireCoOwnerId = litterReg.Sire.CoOwner != null ? litterReg.Sire.CoOwner.Id : 0,
                CoOwnerId = litterReg.Dam.CoOwner != null ? litterReg.Dam.CoOwner.Id : 0,
                Females = litterReg.NumberOfFemalesBeingRegistered,
                Males = litterReg.NumberOfMalesBeingRegistered,
            };
            if (litterReg.CurrentStatus != null && !String.IsNullOrEmpty(litterReg.CurrentStatus.Comments))
            {
                litter.Comments = litterReg.CurrentStatus.Comments;
            }
            _context.Litters.Add(litter);
            litterReg.LitterFromRegistration = litter;
            await _context.SaveChangesAsync();

            //create a new ABKC dog for each puppy in the litter
            for (int i = 0; i < litterReg.NumberOfMalesBeingRegistered; i++)
            {
                BaseDogModel pup = await _dogService.AddPuppyFromLitter(litterReg, litter, BaseDogModel.GenderEnum.Male, createdBy);
            }
            for (int i = 0; i < litterReg.NumberOfFemalesBeingRegistered; i++)
            {
                BaseDogModel pup = await _dogService.AddPuppyFromLitter(litterReg, litter, BaseDogModel.GenderEnum.Female, createdBy);
            }

            return litter;
        }

        public async Task<ICollection<LitterRegistrationModel>> GetLittersByRepresentative(int repId)
        {
            ICollection<LitterRegistrationModel> lst = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.SubmittedBy != null && reg.SubmittedBy.Id == repId).ToListAsync();
            return lst ?? new List<LitterRegistrationModel>();
        }

        public async Task<ICollection<LitterRegistrationModel>> GetRegistrationsByOwner(int ownerId)
        {
            ICollection<LitterRegistrationModel> lst = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Where(r => r.Dam != null && r.Dam.Owner.Id == ownerId).ToListAsync();
            return lst ?? new List<LitterRegistrationModel>();
        }

        public async Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByRepresentative(string searchText)
        {
            ICollection<LitterRegistrationModel> lst = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.SubmittedBy != null
                && reg.SubmittedBy.LoginName.ToLower().Contains(searchText.ToLower())).ToListAsync();
            return lst ?? new List<LitterRegistrationModel>();

        }

        public async Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByDogName(string searchText)
        {
            searchText = searchText.ToLower();
            ICollection<LitterRegistrationModel> lst = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.Sire != null && (reg.Sire.DogName.ToLower().Contains(searchText))
                || (reg.Dam != null && (reg.Dam.DogName.ToLower().Contains(searchText)))
                ).ToListAsync();
            return lst ?? new List<LitterRegistrationModel>();
        }

        public async Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByOwner(string searchText)
        {
            searchText = searchText.ToLower();
            ICollection<LitterRegistrationModel> lst = await _context.LitterRegistrations
                .Include(r => r.Sire)
                .Include(r => r.Sire.Owner)
                .Include(r => r.Dam)
                .Include(r => r.Dam.Owner)
                .Include(r => r.Breed)
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Where(reg => reg.Sire != null && (reg.Sire.Owner != null &&
                reg.Sire.Owner.LastName.ToLower().Contains(searchText)) ||
                (reg.Dam.CoOwner != null && reg.Dam.CoOwner.LastName.ToLower().Contains(searchText))).ToListAsync();
            return lst ?? new List<LitterRegistrationModel>();
        }

        public async Task<bool> DeleteRegistration(int id)
        {
            LitterRegistrationModel reg = await _context.LitterRegistrations.Include(r => r.StatusHistory).FirstOrDefaultAsync(r => r.Id == id);
            if (reg != null)
            {
                if (reg.CurStatus == RegistrationStatusEnum.Approved)
                {
                    throw new InvalidOperationException("Approved registrations cannot be deleted");
                }
                _context.RemoveRange(reg.StatusHistory);
                _context.LitterRegistrations.Remove(reg);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
