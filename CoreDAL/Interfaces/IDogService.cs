using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    public interface IDogService
    {
        Task<ICollection<Dogs>> GetAllDogs();
        IQueryable<Dogs> GetDogsQuery(bool changeTracking = true);
        Task<ICollection<Dogs>> GetDogsMatchingName(string searchTxt);
        Task<Dogs> GetById(string id);
        Task<Dogs> GetById(int id);
        Task<int> AddDog(Dogs newDog);
        Task<int> UpdateDog(Dogs dogToUpdate);
        Task<ICollection<string>> GetAllColorsAsync();
        IQueryable<BaseDogModel> GetDogQuery(int dogId);
        IQueryable<Dogs> GetDogsQueryStartsWith(string searchText);
        Task<int> GetAllDogsCount();
        Task<Dogs> GetByABKCNo(string abkcNumber);
        IQueryable<Dogs> FindByABKCNumberQuery(string abkcNumber);

        #region "New Dog Table Functionality"

        /// <summary>
        /// will retrieve a dog from abkc dogs by id
        /// if it doesn't exist, it will pull from original dogs table and ADD to new ABKC table
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        Task<BaseDogModel> GetDogByOldTableId(int dogId, bool createInNewTable = true);

        Task<BaseDogModel> AddPuppyFromLitter(LitterRegistrationModel litterReg, Litters litter, BaseDogModel.GenderEnum gender, UserModel createdBy);

        IQueryable<BaseDogModel> GetPupsFromLitterQuery(Litters litter);
        Task<BaseDogModel> UpdateFromDTO(BaseDogModel found, BaseDogDTO dogInfo, bool saveChanges);

        #endregion
    }
}