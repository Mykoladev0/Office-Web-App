using AutoMapper;
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
    public class JuniorHandlerService : IJrHandlerService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IDogService _dogService;
        private readonly IMapper _autoMapper;

        public JuniorHandlerService(ABKCOnlineContext context, IDogService dogService, IMapper autoMapper)
        {
            _context = context;
            _dogService = dogService;
            _autoMapper = autoMapper;
        }

        public async Task<IRegistration> CanSubmitRegistration(int registrationId)
        {
            JuniorHandlerRegistrationModel reg = await GetJrRegistrationById(registrationId);
            if (reg == null)
            {
                throw new InvalidOperationException($"Cannot find registration with id {registrationId} to verify");
            }
            return verifyInformationProvided(reg);
        }


        public async Task<JuniorHandlerRegistrationModel> StartJrHandlerRegistration(JuniorHandlerRegistrationDTO reg, UserModel savedBy)
        {
            if (reg == null)
            {
                throw new InvalidOperationException("Jr Handler Registration cannot be empty");
            }
            if (reg.Id > 0)
            {
                throw new InvalidOperationException("Jr Handler Registration already started, use the Save Draft end point");
            }
            if (string.IsNullOrEmpty(reg.FirstName))
            {
                throw new InvalidOperationException("Jr Handler Registration First name must be entered");
            }
            if (string.IsNullOrEmpty(reg.LastName))
            {
                throw new InvalidOperationException("Jr Handler Registration Last Name must be entered");
            }
            //new one!
            JuniorHandlerRegistrationModel toSave = _autoMapper.Map<JuniorHandlerRegistrationModel>(reg);
            _context.JuniorHandlerRegistrations.Add(toSave);
            await _context.SaveChangesAsync();
            return toSave;

        }

        public async Task<JuniorHandlerRegistrationModel> StartDraftRegistration(JuniorHandlerRegistrationDTO registration, UserModel savedBy)
        {
            if (registration.Id > 0)
            {
                throw new InvalidOperationException($"registration has already been started for that id, use SaveDraftRegistration");
            }
            JuniorHandlerRegistrationModel toSave = new JuniorHandlerRegistrationModel
            {

            };
            toSave = populateFromDTO(registration, toSave);
            toSave.SetStatus(RegistrationStatusEnum.Draft, savedBy);
            _context.JuniorHandlerRegistrations.Add(toSave);
            await _context.SaveChangesAsync();
            return toSave;
        }
        public async Task<JuniorHandlerRegistrationModel> SaveDraftJrRegistration(JuniorHandlerRegistrationDTO reg, UserModel savedBy)
        {
            //check to see if it has a valid id, makes it an update
            JuniorHandlerRegistrationModel toSave = null;

            //existing, need logic :)
            toSave = await _context.JuniorHandlerRegistrations.FirstOrDefaultAsync(r => r.Id == reg.Id);
            if (toSave == null)
            {
                return null;
                // throw new InvalidOperationException($"No junior handler registration with Id {reg.Id} could be found");
            }
            // JuniorHandlerRegistrationModel newData = _autoMapper.Map<JuniorHandlerRegistrationModel>(registration);
            toSave = populateFromDTO(reg, toSave);
            //we are back in draft status!
            if (toSave.CurrentStatus == null || toSave.CurrentStatus.Status != RegistrationStatusEnum.Draft)
            {
                toSave.SetStatus(RegistrationStatusEnum.Draft, savedBy);
            }
            await _context.SaveChangesAsync();
            return toSave;
        }

        private JuniorHandlerRegistrationModel populateFromDTO(JuniorHandlerRegistrationDTO reg, JuniorHandlerRegistrationModel toSave)
        {
            if (!string.IsNullOrEmpty(reg.FirstName))
            {
                toSave.FirstName = reg.FirstName;
            }
            if (!string.IsNullOrEmpty(reg.LastName))
            {
                toSave.LastName = reg.LastName;
            }
            if (!string.IsNullOrEmpty(reg.ParentFirstName))
            {
                toSave.ParentFirstName = reg.ParentFirstName;
            }
            if (!string.IsNullOrEmpty(reg.ParentLastName))
            {
                toSave.ParentLastName = reg.ParentLastName;
            }
            if (!string.IsNullOrEmpty(reg.Address1))
            {
                toSave.Address1 = reg.Address1;
            }
            if (!string.IsNullOrEmpty(reg.Address2))
            {
                toSave.Address2 = reg.Address2;
            }
            if (!string.IsNullOrEmpty(reg.Address3))
            {
                toSave.Address3 = reg.Address3;
            }
            if (!string.IsNullOrEmpty(reg.Cell))
            {
                toSave.Cell = reg.Cell;
            }
            if (!string.IsNullOrEmpty(reg.City))
            {
                toSave.City = reg.City;
            }
            if (!string.IsNullOrEmpty(reg.Country))
            {
                toSave.Country = reg.Country;
            }
            if (!string.IsNullOrEmpty(reg.State))
            {
                toSave.State = reg.State;
            }
            if (!string.IsNullOrEmpty(reg.Zip))
            {
                toSave.Zip = reg.Zip;
            }
            if (!string.IsNullOrEmpty(reg.Phone))
            {
                toSave.Phone = reg.Phone;
            }
            if (!string.IsNullOrEmpty(reg.Email))
            {
                toSave.Email = reg.Email;
            }
            if (reg.DateOfBirth != null && reg.DateOfBirth.HasValue)
            {
                toSave.DateOfBirth = reg.DateOfBirth;
            }
            if (reg.International != null && reg.International.HasValue)
            {
                toSave.International = reg.International;
            }
            toSave.IsInternationalRegistration = reg.IsInternational;
            toSave.OvernightRequested = reg.OvernightRequested.HasValue ? reg.OvernightRequested.Value : toSave.OvernightRequested;
            toSave.RushRequested = reg.RushRequested.HasValue ? reg.RushRequested.Value : toSave.RushRequested;

            return toSave;
        }


        public async Task<JuniorHandlerRegistrationModel> GetJrRegistrationById(int id)
        {
            JuniorHandlerRegistrationModel found = await _context.JuniorHandlerRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .Include(r => r.AssociatedTransaction)
                .OrderBy(r => r.DateModified)
                .FirstOrDefaultAsync(r => r.Id == id);
            return found;
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> GetAllJrRegistrations()
        {
            ICollection<JuniorHandlerRegistrationModel> list = await _context.JuniorHandlerRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified).ToListAsync();
            return list;
        }

        public ICollection<SupportingDocumentTypeEnum> GetJrHandlerDocsProvided(int regId)
        {
            List<SupportingDocumentTypeEnum> rtn = new List<SupportingDocumentTypeEnum>();
            // var found = _context.LitterRegistrations.Where(r => r.Id == id && r.SireOwnerSignature != null).Select(r => r.SireOwnerSignature.Id);
            // if (found != null && found.Any())
            // {
            //     rtn.Add(SupportingDocumentTypeEnum.SireOwnerSignature);
            // }
            // found = _context.LitterRegistrations.Where(r => r.Id == id && r.SireCoOwnerSignature != null).Select(r => r.SireCoOwnerSignature.Id);
            // if (found != null && found.Any())
            // {
            //     rtn.Add(SupportingDocumentTypeEnum.SireCoOwnerSignature);
            // }
            // found = _context.LitterRegistrations.Where(r => r.Id == id && r.DamOwnerSignature != null).Select(r => r.DamOwnerSignature.Id);
            // if (found != null && found.Any())
            // {
            //     rtn.Add(SupportingDocumentTypeEnum.DamOwnerSignature);
            // }
            // found = _context.LitterRegistrations.Where(r => r.Id == id && r.DamCoOwnerSignature != null).Select(r => r.DamCoOwnerSignature.Id);
            // if (found != null && found.Any())
            // {
            //     rtn.Add(SupportingDocumentTypeEnum.DamCoOwnerSignature);
            // }

            return rtn;
        }

        public async Task<JuniorHandlerRegistrationModel> ApproveRegistration(JuniorHandlerRegistrationModel reg, UserModel approvedBy)
        {
            reg = verifyInformationProvided(reg);
            //build a jr handler in the system
            await createJrHandlerFromReg(reg, approvedBy);
            return reg;
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null)
        {
            var q = _context.JuniorHandlerRegistrations
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
            return l ?? new List<JuniorHandlerRegistrationModel>();
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> GetByRepresentative(int repId)
        {
            ICollection<JuniorHandlerRegistrationModel> list = await _context.JuniorHandlerRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified)
                .Where(reg => reg.SubmittedBy != null && reg.SubmittedBy.Id == repId).ToListAsync();
            return list;
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> GetRegistrationsByOwner(int ownerId) => new List<JuniorHandlerRegistrationModel>();

        public async Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByRepresentative(string searchText)
        {
            ICollection<JuniorHandlerRegistrationModel> list = await _context.JuniorHandlerRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified)
                .Where(reg => reg.SubmittedBy != null
                && reg.SubmittedBy.LoginName.ToLower().Contains(searchText.ToLower())).ToListAsync();
            return list ?? new List<JuniorHandlerRegistrationModel>();
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByDogName(string searchText)
        {
            ICollection<JuniorHandlerRegistrationModel> list = await _context.JuniorHandlerRegistrations
                .Include(r => r.StatusHistory)
                .Include(r => r.SubmittedBy)
                .Include(r => r.SubmittedBy.Roles)
                .OrderBy(r => r.DateModified)
                .Where(reg => reg.FirstName.ToLower().Contains(searchText) || reg.LastName.ToLower().Contains(searchText)
                || reg.ParentFirstName.ToLower().Contains(searchText) || reg.ParentLastName.ToLower().Contains(searchText)).ToListAsync();

            return list ?? new List<JuniorHandlerRegistrationModel>();
        }

        public async Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByOwner(string searchText) => new List<JuniorHandlerRegistrationModel>();


        public async Task<bool> DeleteRegistration(int id)
        {
            try
            {
                JuniorHandlerRegistrationModel reg = await _context.JuniorHandlerRegistrations.Include(r => r.StatusHistory).FirstOrDefaultAsync(r => r.Id == id);
                if (reg != null)
                {
                    if (reg.CurStatus == RegistrationStatusEnum.Approved)
                    {
                        throw new InvalidOperationException("Approved registrations cannot be deleted");
                    }
                    _context.RemoveRange(reg.StatusHistory);
                    _context.JuniorHandlerRegistrations.Remove(reg);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private JuniorHandlerRegistrationModel verifyInformationProvided(JuniorHandlerRegistrationModel reg)
        {
            ICollection<SupportingDocumentTypeEnum> docs = GetJrHandlerDocsProvided(reg.Id);

            if (string.IsNullOrEmpty(reg.FirstName))
            {
                throw new InvalidOperationException("Junior Handler's first name required to submit a registration");
            }
            if (string.IsNullOrEmpty(reg.LastName))
            {
                throw new InvalidOperationException("Junior Handler's last name required to submit a registration");
            }

            if (string.IsNullOrEmpty(reg.ParentFirstName))
            {
                throw new InvalidOperationException("Junior Handler Parent's first name required to submit a registration");
            }
            if (string.IsNullOrEmpty(reg.ParentLastName))
            {
                throw new InvalidOperationException("Junior Handler Parent's last name required to submit a registration");
            }
            if (string.IsNullOrEmpty(reg.Phone))
            {
                throw new InvalidOperationException("Junior Handler Parent's phone number required to submit a registration");
            }
            if (string.IsNullOrEmpty(reg.Email))
            {
                throw new InvalidOperationException("Junior Handler Parent's email address required to submit a registration");
            }
            return reg;
        }


        private async Task<JrHandlers> createJrHandlerFromReg(JuniorHandlerRegistrationModel reg, UserModel createdBy)
        {

            //TODO:first make sure that the Junior handler doesn't already exist

            //create new junior handler
            JrHandlers handler = new JrHandlers
            {
                ChildFirstName = reg.FirstName,
                ChildLastName = reg.LastName,
                Email = reg.Email,
                International = reg.IsInternationalRegistration,
                Dob = reg.DateOfBirth,
                ParentFirstName = reg.ParentFirstName,
                ParentLastName = reg.ParentLastName,
                Address1 = reg.Address1,
                Address2 = reg.Address2,
                Address3 = reg.Address3,
                Cell = reg.Cell,
                Country = reg.Country,
                CreateDate = DateTime.UtcNow,
                ModifiedBy = createdBy.LoginName,
                City = reg.City,
                State = reg.State,
                Zip = reg.Zip
            };
            if (reg.CurrentStatus != null && !String.IsNullOrEmpty(reg.CurrentStatus.Comments))
            {
                handler.Comments = reg.CurrentStatus.Comments;
            }
            _context.JrHandlers.Add(handler);
            await _context.SaveChangesAsync();
            return handler;
        }

    }
}
