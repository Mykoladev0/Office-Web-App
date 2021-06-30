using ABKCCommon.Models.DTOs;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using CoreDAL.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class OwnerService : IOwnerService
    {
        private ABKCOnlineContext _context;

        public OwnerService(ABKCOnlineContext context)
        {
            _context = context;
        }

        public IQueryable<Owners> GetOwnersQuery(bool changeTracking = true) => changeTracking ? _context.Owners : _context.Owners.AsNoTracking();

        public IQueryable<Owners> GetOwnersQueryStartsWith(string searchText)
        {
            IQueryable<Owners> q = null;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-US");
            if (Int32.TryParse(searchText, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int number))
            {
                q = _context.Owners.Where(o => o.FirstName.ToLower().StartsWith(searchText.ToLower()) ||
                    o.LastName.ToLower().StartsWith(searchText.ToLower()) || o.FullName.ToLower().StartsWith(searchText.ToLower()) || (o.OwnerId.ToString().StartsWith(searchText)));
            }
            else
            {
                if (Validators.IsValidEmail(searchText))
                {
                    q = _context.Owners.Where(o => o.Email.ToLower().Contains(searchText.ToLower()));
                }
                else
                {
                    q = _context.Owners.Where(o => o.FirstName.ToLower().StartsWith(searchText.ToLower()) ||
                        o.LastName.ToLower().StartsWith(searchText.ToLower()) || o.FullName.ToLower().StartsWith(searchText.ToLower()));
                }
            }
            return q;
        }

        public Task<Owners> GetById(string id)
        {
            if (!int.TryParse(id, out int nId))
            {
                throw new ArgumentException("id is not an integer");
            }
            return GetById(nId);
        }
        public async Task<Owners> GetById(int id)
        {
            if (id <= 1)
            {
                return null;
            }
            Owners o = await _context.Owners.FindAsync(id);
            return o;
        }
        public async Task<int> GetOwnersCount()
        {
            return await _context.Owners.CountAsync();
        }

        public async Task<Owners> AddOwnerWithoutFullNameWrite(Owners ownerToAdd, UserModel modifiedBy)
        {
            try
            {
                if (modifiedBy != null)
                {
                    ownerToAdd.ModifiedBy = modifiedBy.LoginName;
                }
                if (string.IsNullOrEmpty(ownerToAdd.ModifiedBy))
                {
                    //modifiedby needs to be set, eventually this will happen in service with user context?
                    throw new InvalidOperationException("ModifiedBy field needs to be set");
                }
                if (string.IsNullOrEmpty(ownerToAdd.Email))
                {
                    //modifiedby needs to be set, eventually this will happen in service with user context?
                    throw new InvalidOperationException("Email Address field needs to be set");
                }
                if (string.IsNullOrEmpty(ownerToAdd.LastName))
                {
                    //modifiedby needs to be set, eventually this will happen in service with user context?
                    throw new InvalidOperationException("Owner Last Name field needs to be set");
                }
                //check to see if owner with same email exists?
                bool ownerExists = _context.Owners.Where(o => o.Email.ToLower() == ownerToAdd.Email.ToLower()).Any();
                if (ownerExists)
                {
                    throw new Exception("Owner with the email already exists");
                }

                //get the next owner Id
                int count = await _context.Owners.CountAsync();
                int currentOwnerId = count > 0 ? (await _context.Owners.MaxAsync(d => d.OwnerId)) : 0;
                ownerToAdd.OwnerId = currentOwnerId + 1;//getting errors on saving (test db);

                DateTime insertDate = DateTime.UtcNow;
                ownerToAdd.CreateDate = insertDate;
                ownerToAdd.ModifyDate = insertDate;

                var a = await _context.Owners.AddAsync(ownerToAdd);

                _context.SaveChanges();
                return ownerToAdd;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Owners> AddOwner(Owners ownerToAdd, UserModel modifiedBy)
        {
            try
            {
                var addedOwner = await AddOwnerWithoutFullNameWrite(ownerToAdd, modifiedBy);
                await SetFullNameForOwner(addedOwner);
                return addedOwner;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Owners> UpdateOwnerWithoutFullNameWrite(Owners originalOwner, Owners ownerToUpdate, UserModel modifiedBy)
        {
            //check to see if owner with same email exists?
            try
            {
                if (originalOwner == null)
                {
                    throw new Exception($"Cannot update an owner that does not exist in the system with ID: {ownerToUpdate.Id}");
                }
                //do some validation checks here!  Probably some data normalization as well
                ownerToUpdate.ModifyDate = DateTime.UtcNow;
                if (modifiedBy != null)
                {
                    ownerToUpdate.ModifiedBy = modifiedBy.LoginName;
                }
                // ownerToUpdate.ModifiedBy = "Server USER";
                //TODO: research what happens with navigation properties once those are added
                _context.Entry(originalOwner).CurrentValues.SetValues(ownerToUpdate);
                await _context.SaveChangesAsync();
                return originalOwner;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<Owners> UpdateOwner(Owners ownerToUpdate, UserModel modifiedBy)
        {
            //check to see if owner with same email exists?
            try
            {
                Owners originalOwner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == ownerToUpdate.Id);
                bool nameChanged = false;
                nameChanged = originalOwner.FirstName != ownerToUpdate.FirstName;
                nameChanged = nameChanged || originalOwner.MiddleInitial != ownerToUpdate.MiddleInitial;
                nameChanged = nameChanged || originalOwner.LastName != ownerToUpdate.LastName;
                await UpdateOwnerWithoutFullNameWrite(originalOwner, ownerToUpdate, modifiedBy);
                if (nameChanged)
                {
                    await SetFullNameForOwner(ownerToUpdate);
                }
                return originalOwner;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Owners> UpdateFromDTO(Owners owner, OwnerDTO ownerEdits, bool saveChanges)
        {
            if (owner == null || ownerEdits == null)
            {
                return owner;
            }
            bool nameChanged = false;
            if (!string.IsNullOrEmpty(ownerEdits.FirstName))
            {
                owner.FirstName = ownerEdits.FirstName;
                nameChanged = true;
            }
            if (!string.IsNullOrEmpty(ownerEdits.LastName))
            {
                owner.LastName = ownerEdits.LastName;
                nameChanged = true;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Address1))
            {
                owner.Address1 = ownerEdits.Address1;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Address2))
            {
                owner.Address2 = ownerEdits.Address2;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Address3))
            {
                owner.Address3 = ownerEdits.Address3;
            }
            if (!string.IsNullOrEmpty(ownerEdits.State))
            {
                owner.State = ownerEdits.State;
            }
            if (!string.IsNullOrEmpty(ownerEdits.City))
            {
                owner.City = ownerEdits.City;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Country))
            {
                owner.Country = ownerEdits.Country;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Email))
            {
                owner.Email = ownerEdits.Email;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Zip))
            {
                owner.Zip = ownerEdits.Zip;
            }
            if (!string.IsNullOrEmpty(ownerEdits.Phone))
            {
                owner.Phone = ownerEdits.Phone;
            }
            if (ownerEdits.International)
            {
                owner.International = ownerEdits.International;
            }
            if (saveChanges)
            {
                if (nameChanged)
                {
                    await SetFullNameForOwner(owner);
                }
                await _context.SaveChangesAsync();
            }

            return owner;
        }


        private async Task<Owners> SetFullNameForOwner(Owners owner)
        {
            //set full name UGGGH! in db for backwards compatability
            List<string> names = new List<string> { owner.FirstName, owner.MiddleInitial ?? "", owner.LastName };
            string fullName = String.Join(" ", names.Where(n => !String.IsNullOrEmpty(n)));
            if (_context.Database.IsSqlServer())
            {
                RawSqlString sql = new RawSqlString($"Update [dbo].[Owners] SET [FullName] = '{fullName}' WHERE [Owner_Id]={owner.OwnerId}");
#pragma warning disable EF1000 // Possible SQL injection vulnerability.
                await _context.Database.ExecuteSqlCommandAsync(sql);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
            }
            return owner;
        }

    }
}
