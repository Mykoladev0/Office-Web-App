using AutoMapper;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class DogService : IDogService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IOwnerService _ownerService;
        private readonly IMapper _autoMapper;

        public DogService(ABKCOnlineContext context, IMapper autoMapper, IOwnerService ownerService)
        {
            _context = context;
            _ownerService = ownerService;
            _autoMapper = autoMapper;
        }

        public async Task<ICollection<Dogs>> GetAllDogs()
        {
            return await _context.Dogs.ToListAsync();
        }

        public IQueryable<Dogs> GetDogsQuery(bool changeTracking = true) => changeTracking ? _context.Dogs : _context.Dogs.AsNoTracking();

        public async Task<ICollection<Dogs>> GetDogsMatchingName(string searchTxt)
        {
            var q = _context.Dogs.Where(d => d.DogName.ToLower().Contains(searchTxt.ToLower()));
            return await q.ToListAsync();
        }
        public Task<Dogs> GetById(string id)
        {
            if (!int.TryParse(id, out int nId))
            {
                throw new ArgumentException("id is not an integer");
            }
            return GetById(nId);
        }
        public async Task<Dogs> GetById(int id)
        {
            if (id <= 1) return null;//1 is not valid in old system
            Dogs d = await _context.Dogs.FindAsync(id);
            return d;
        }
        public async Task<Dogs> GetByABKCNo(string abkcNumber)
        {
            if (string.IsNullOrEmpty(abkcNumber))
            {
                return null;
            }

            if (!abkcNumber.Contains(','))
            {
                if (!int.TryParse(abkcNumber, out int num))
                {
                    throw new InvalidOperationException("abkc number is not valid!");
                }
                //add it!
                abkcNumber = $"{num:n0}";
            }
            Dogs found = await _context.Dogs.FirstOrDefaultAsync(d => d.AbkcNo == abkcNumber);
            return found;
        }
        public IQueryable<Dogs> FindByABKCNumberQuery(string abkcNumber)
        {
            if (string.IsNullOrEmpty(abkcNumber))
            {
                return null;
            }

            if (!abkcNumber.Contains(','))
            {
                if (!int.TryParse(abkcNumber, out int num))
                {
                    throw new InvalidOperationException("abkc number is not valid!");
                }
                //add it!
                abkcNumber = $"{num:n0}";
            }
            return _context.Dogs.Where(d => d.AbkcNo.Contains(abkcNumber));
        }

        public async Task<int> AddDog(Dogs newDog)
        {
            try
            {
                newDog.Id = 0;
                if (string.IsNullOrEmpty(newDog.ModifiedBy))
                {
                    //modifiedby needs to be set, eventually this will happen in service with user context?
                    throw new InvalidOperationException("ModifiedBy field needs to be set");
                }
                //check to see if dog with same name exists?
                bool dogExists = _context.Dogs.Where(d => d.DogName == newDog.DogName && newDog.OwnerId == d.OwnerId && d.OwnerId != -1).Any();
                if (dogExists)
                {
                    throw new Exception("Dog with name and owner already exists");
                }

                //get the next bully Id
                int count = await _context.Dogs.CountAsync();
                int currentBullyId = count > 0 ? (await _context.Dogs.MaxAsync(d => d.BullyId)) : 0;
                newDog.BullyId = currentBullyId + 1;
                await GetABKCNumber(newDog);
                newDog.DogName = !String.IsNullOrEmpty(newDog.DogName) && newDog.DogName.Length > 50 ? newDog.DogName.Substring(0, 50) : newDog.DogName;
                if (string.IsNullOrEmpty(newDog.DogName))
                {
                    //puppy!
                    int num = int.Parse(newDog.AbkcNo.Replace(",", ""));
                    newDog.DogName = $"(unregistered puppy #) {num}";
                }
                newDog.Breed = !String.IsNullOrEmpty(newDog.Breed) && newDog.Breed.Length > 30 ? newDog.Breed.Substring(0, 30) : newDog.Breed;
                DateTime insertDate = DateTime.UtcNow;
                newDog.DateCreated = insertDate;
                newDog.LastModified = insertDate;
                newDog = ChangeQuestionGenderToUnknown(newDog);
                var a = await _context.Dogs.AddAsync(newDog);
                //_context.Entry(newDog).State = EntityState.Added;
                _context.SaveChanges();
                return newDog.Id;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> UpdateDog(Dogs dogToUpdate)
        {
            //check to see if dog with same name exists?
            try
            {
                Dogs originalDog = await _context.Dogs.FirstOrDefaultAsync(d => d.Id == dogToUpdate.Id);
                if (originalDog == null)
                {
                    throw new Exception($"Cannot update a dog that does not exist in the system with ID: {dogToUpdate.Id}");
                }

                //handle WEIRD Case where BullyId is null
                if (originalDog.BullyId <= 0 && dogToUpdate.BullyId <= 0)
                {
                    int currentBullyId = (await _context.Dogs.MaxAsync(d => d.BullyId));
                    dogToUpdate.BullyId = currentBullyId + 1;
                }

                if (string.IsNullOrEmpty(originalDog.AbkcNo) ||
                    string.IsNullOrEmpty(dogToUpdate.AbkcNo) ||
                    originalDog.AbkcNo != dogToUpdate.AbkcNo)
                {
                    await GetABKCNumber(dogToUpdate);
                }
                dogToUpdate = ChangeQuestionGenderToUnknown(dogToUpdate);
                dogToUpdate.LastModified = DateTime.UtcNow;
                //TODO: research what happens with navigation properties once those are added
                _context.Entry(originalDog).CurrentValues.SetValues(dogToUpdate);
                await _context.SaveChangesAsync();
                return originalDog.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<string>> GetAllColorsAsync()
        {
            ICollection<string> colors = await _context.Dogs.Where(d => !string.IsNullOrEmpty(d.Color))
                .Select(d => d.Color).Distinct().OrderBy(c => c).ToListAsync();
            return colors;
        }

        public IQueryable<Dogs> GetDogsQueryStartsWith(string searchText)
        {
            IQueryable<Dogs> q = null;
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("en-US");
            if (Int32.TryParse(searchText, NumberStyles.Integer | NumberStyles.AllowThousands, provider, out int number))
            {
                q = _context.Dogs.Where(d => d.DogName.ToLower().StartsWith(searchText.ToLower()) || d.AbkcNo.StartsWith(searchText) || (d.BullyId.ToString().StartsWith(searchText)));
            }
            else
            {
                q = _context.Dogs.Where(d => d.DogName.ToLower().StartsWith(searchText.ToLower()));
            }
            return q;
        }

        public async Task<int> GetAllDogsCount()
        {
            //I assume we will want to filter out dogs without names, bully ids, etc?
            return await _context.Dogs.CountAsync();
        }

        #region Helper Methods

        private static Dogs ChangeQuestionGenderToUnknown(Dogs dog)
        {
            if (string.IsNullOrEmpty(dog.Gender) || dog.Gender.Contains("?"))
            {
                dog.Gender = "Unknown";
            }
            return dog;
        }

        private async Task GetABKCNumber(Dogs dog)
        {
            if (!string.IsNullOrEmpty(dog.AbkcNo))
            {
                //get right format
                string abkcNo = dog.AbkcNo;
                if (abkcNo.Length < 7)
                    abkcNo = abkcNo.PadRight(7, '0');

                if (abkcNo.Length > 7)
                    abkcNo = abkcNo.Substring(0, 7);
                int commaIndex = abkcNo.IndexOf(',');
                if (commaIndex != 3)
                {
                    Regex rgx = new Regex(@"\D");
                    abkcNo = rgx.Replace(abkcNo, "");
                    if (abkcNo.Length < 6)
                        abkcNo = abkcNo.PadRight(6, '0');
                    abkcNo = Int32.Parse(abkcNo).ToString("000,000");
                }
                //see if abkcno exists, if so, use bully ID?
                bool exists = await _context.Dogs.Where(d => d.AbkcNo == abkcNo).AnyAsync();
                if (exists)
                {
                    abkcNo = dog.BullyId.ToString("000,000");
                }
                if (abkcNo.StartsWith('0'))
                {
                    //should never happen, but backup case
                    StringBuilder sb = new StringBuilder(abkcNo);
                    sb[1] = '1';
                    abkcNo = sb.ToString();
                }
                dog.AbkcNo = abkcNo;
            }
            else
            {
                string abkcNo = dog.BullyId.ToString("000,000");
                if (abkcNo.StartsWith('0'))
                {
                    //should never happen, but backup case
                    StringBuilder sb = new StringBuilder(abkcNo);
                    sb[0] = '1';
                    abkcNo = sb.ToString();
                }
                dog.AbkcNo = abkcNo;
            }
        }

        #endregion


        #region "New Dog Table Functionality"

        /// <summary>
        /// returns a new Dog Model
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        public IQueryable<BaseDogModel> GetDogQuery(int dogId)
        {
            //tries to get from new system
            IQueryable<BaseDogModel> found = _context.ABKCDogs.Where(d => d.Id == dogId);
            if (found.Any())
            {
                return found;
            }
            //grab data from old system, map to new model (with ancestry uggh!), convert to query
            //DEFINITE TODO!
            return Enumerable.Empty<BaseDogModel>().AsQueryable();
        }

        /// <summary>
        /// will retrieve a dog from abkc dogs by id
        /// if it doesn't exist, it will pull from original dogs table and ADD to new ABKC table
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        public async Task<BaseDogModel> GetDogByOldTableId(int dogId, bool createInNewTable = true)
        {
            BaseDogModel abkcSire = await _context.ABKCDogs.FirstOrDefaultAsync(d => d.OriginalDogTableId == dogId);
            if (abkcSire != null)
            {
                return abkcSire;
            }
            else
            {
                Dogs sire = await _context.Dogs.FirstOrDefaultAsync(c => c.Id == dogId);
                if (sire != null)
                {
                    BaseDogModel origSire = _autoMapper.Map<BaseDogModel>(sire);
                    if (origSire.Owner != null && origSire.Owner.Id == 0)
                    {
                        origSire.Owner = null;
                    }
                    else
                    {
                        origSire.Owner = await _ownerService.GetById(origSire.Owner.Id);
                    }
                    if (origSire.CoOwner != null && origSire.CoOwner.Id == 0)
                    {
                        origSire.CoOwner = null;
                    }
                    else
                    {
                        origSire.CoOwner = await _ownerService.GetById(origSire.CoOwner.Id);
                    }
                    origSire.Id = 0;

                    if (origSire.Breed == null && !string.IsNullOrEmpty(sire.Breed))
                    {
                        origSire.Breed = await _context.Breeds.FirstOrDefaultAsync(b => b.Breed.ToLower() == sire.Breed);
                    }
                    if (origSire.Color == null && !string.IsNullOrEmpty(sire.Color))
                    {
                        origSire.Color = await _context.Colors.FirstOrDefaultAsync(b => b.Color.ToLower() == sire.Color);
                    }

                    if (createInNewTable)
                    {
                        await _context.ABKCDogs.AddAsync(origSire);
                    }

                    return origSire;
                }
            }
            return null;
        }

        public async Task<BaseDogModel> AddPuppyFromLitter(LitterRegistrationModel litterReg, Litters litter, BaseDogModel.GenderEnum gender, UserModel createdBy)
        {

            Dogs dog = new Dogs
            {
                DogName = "",
                OwnerId = litterReg.Dam.Owner != null ? litterReg.Dam.Owner.Id : 0,
                CoOwnerId = litterReg.Dam.CoOwner != null ? litterReg.Dam.CoOwner.Id : 0,
                Gender = gender.ToString(),
                SireNo = litterReg.Sire.Id,
                DamNo = litterReg.Dam.Id,
                Breed = litterReg.Breed.Breed,
                Birthdate = litterReg.DateOfLitterBirth.Value,
                LitterNo = litter.Id,
                ModifiedBy = createdBy?.LoginName
            };
            await AddDog(dog);
            //create new ABKC dog now

            BaseDogModel newPup = new BaseDogModel
            {
                DogName = dog.DogName,
                ABKCNumber = dog.AbkcNo,
                Owner = litterReg.Dam.Owner,
                CoOwner = litterReg.Dam.CoOwner,
                Gender = gender,
                Sire = litterReg.Sire,
                Dam = litterReg.Dam,
                Breed = litterReg.Breed,
                DateOfBirth = litterReg.DateOfLitterBirth.Value,
                Litter = litter,
                OriginalDogTableId = dog.Id,
                OwnerSignature = litterReg.DamOwnerSignature,
                CoOwnerSignature = litterReg.DamCoOwnerSignature,
                LastModifiedBy = createdBy
                //TODO:add lastmodifiedby field
            };
            _context.ABKCDogs.Add(newPup);
            await _context.SaveChangesAsync();
            return newPup;
        }

        public IQueryable<BaseDogModel> GetPupsFromLitterQuery(Litters litter)
        {
            return _context.ABKCDogs
            .Where(d => d.Litter != null && d.Litter.Id == litter.Id);
        }

        public async Task<BaseDogModel> UpdateFromDTO(BaseDogModel found, BaseDogDTO dogInfo, bool saveChanges)
        {
            if (found == null || dogInfo == null)
            {
                return found;
            }

            if (!string.IsNullOrEmpty(dogInfo.DogName))
            {
                found.DogName = dogInfo.DogName;
            }
            if (!string.IsNullOrEmpty(dogInfo.Gender))
            {
                if (Enum.TryParse(dogInfo.Gender, true, out BaseDogModel.GenderEnum genderParse))
                {
                    found.Gender = genderParse;
                }
            }
            if (!string.IsNullOrEmpty(dogInfo.MicrochipNumber))
            {
                found.MicrochipNumber = dogInfo.MicrochipNumber;
            }

            if (dogInfo.DateOfBirth.HasValue && dogInfo.DateOfBirth.Value != DateTime.MinValue)
            {
                found.DateOfBirth = dogInfo.DateOfBirth.Value;
            }
            if (dogInfo.DamId.HasValue && dogInfo.DamId > 0)
            {
                found.Dam = await GetDogByOldTableId(dogInfo.DamId.Value);
            }
            if (dogInfo.SireId.HasValue && dogInfo.SireId > 0)
            {
                found.Sire = await GetDogByOldTableId(dogInfo.SireId.Value);
            }
            if (dogInfo.OwnerId > 0)
            {
                found.Owner = await _ownerService.GetById(dogInfo.OwnerId);
            }
            if (dogInfo.CoOwnerId.HasValue && dogInfo.CoOwnerId > 0)
            {
                found.CoOwner = await _ownerService.GetById(dogInfo.CoOwnerId.Value);
            }
            if (dogInfo.BreedId > 0)
            {
                found.Breed = await _context.Breeds.Where(b => b.Id == dogInfo.BreedId).FirstOrDefaultAsync();
            }
            if (dogInfo.ColorId > 0)
            {
                found.Color = await _context.Colors.Where(b => b.Id == dogInfo.ColorId).FirstOrDefaultAsync();
            }
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return found;
        }

        #endregion
    }
}
