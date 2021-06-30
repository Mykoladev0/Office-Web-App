using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    /// <summary>
    /// used to work with pedigree, puppy, bullyid, and transfer requests
    /// </summary>
    public interface IDogRegistrationService
    {
        #region "Pedigree"
        Task<RegistrationModel> SaveDraftPedigreeRegistration(PedigreeRegistrationDraftDTO registration, UserModel savedBy);

        Task<RegistrationModel> StartDraftPedigreeRegistration(BaseDogDTO dogInfo, UserModel user);
        /// <summary>
        /// TODO:Add swipe information to track payment
        /// NOTE! Multiple registrations (and types) can tie to one payment, need separate payment table
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="isInternationRegistration"></param>
        /// <param name="rushRequested"></param>
        /// <param name="overnightRequested"></param>
        /// <returns></returns>

        Task<ICollection<RegistrationModel>> GetAllPedigreeRegistrations();
        Task<RegistrationModel> GetPedigreeRegistrationByIdAsync(int id);
        Task<IRegistration> CanSubmitPedigreeRegistration(int regId);
        Task<IRegistration> ApprovePedigreeRegistration(RegistrationModel reg, UserModel user);
        IQueryable<RegistrationModel> GetPedigreeRegistrationsByRepresentativeQuery(int repId);

        IQueryable<RegistrationModel> GetPedigreeRegistrationsByOwnerQuery(int ownerId);
        IQueryable<RegistrationModel> SearchPedigreeRegistrationsByRepresentativeQuery(string searchText);
        IQueryable<RegistrationModel> SearchPedigreeRegistrationsByOwner(string searchText);

        IQueryable<RegistrationModel> SearchPedigreeRegistrationsByDogName(string searchText);
        Task<ICollection<RegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel user = null);
        ICollection<SupportingDocumentTypeEnum> GetPedigreeDocsProvided(int id);
        Task<AttachmentModel> GetSupportingPedigreeDocument(int id, SupportingDocumentTypeEnum documentType);


        Task<bool> DeletePedigreeRegistration(int id);

        #endregion

        #region "BullyId Requests"

        Task<BullyIdRequestModel> GetBullyIdRequestForDogId(int dogId);
        Task<BullyIdRequestModel> CreateBullyIdRequest(int dogId, UserModel user);
        Task<bool> DeleteBullyIdRequest(int id);
        Task<BullyIdRequestModel> GetBullyIdRequestById(int id);
        Task<ICollection<PuppyRegistrationModel>> GetAllPuppyRegistrations();
        #endregion

        #region Puppy Registrations
        /// <summary>
        /// retrieves the dog by id and starts a puppy registration if that dog is eligable
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        Task<PuppyRegistrationModel> StartPuppyRegistration(int dogId, UserModel savedBy, bool isTransferRequest);
        Task<ICollection<BullyIdRequestModel>> GetAllBullyIdRequests();


        /// <summary>
        /// updates an existing puppy registration draft will new/changed information provided in the registration dto
        /// note: only microchip and color dog information may be updated
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="reg"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<PuppyRegistrationModel> SavePuppyDraft(int regId, PuppyRegistrationDraftDTO reg, UserModel user);

        /// <summary>
        /// retrieves a puppy registration by dogId searching new and old tables
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        Task<PuppyRegistrationModel> GetPuppyRegistrationForDogId(int dogId);
        Task<PuppyRegistrationModel> GetPuppyRegistrationByIdAsync(int id);
        ICollection<SupportingDocumentTypeEnum> GetPuppyDocsProvided(int id);
        Task<AttachmentModel> GetSupportingPuppyDocument(int id, SupportingDocumentTypeEnum documentType);
        Task<bool> DeletePuppyRegistration(int id);

        Task<IRegistration> CanSubmitPuppyRegistration(int regId);
        Task<IRegistration> ApprovePuppyRegistration(PuppyRegistrationModel reg, UserModel user);
        Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByRepresentative(int repId);
        Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByOwner(int ownerId);
        Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByRepresentative(string searchText);
        Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByOwner(string searchText);
        Task<ICollection<PuppyRegistrationModel>> SearchPuppyRegistrationsByDogName(string searchText);
        Task<ICollection<PuppyRegistrationModel>> GetPuppyRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null);
        Task<ICollection<BullyIdRequestModel>> GetBullyIdRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null);
        Task<ICollection<BullyIdRequestModel>> GetBullyRequestsByRepresentative(int repId);
        Task<ICollection<BullyIdRequestModel>> GetBullyRequestsByOwner(int ownerId);
        Task<IRegistration> ApproveBullyIdRequest(BullyIdRequestModel bullyIdRequestModel, UserModel user);
        Task<AttachmentModel> GetSupportingBullyIdDocument(int id, SupportingDocumentTypeEnum documentType);
        Task<bool> AddSupportingBullyIdRegistrationDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, UserModel savedBy);
        ICollection<SupportingDocumentTypeEnum> GetBullyIdDocsProvided(int id);

        #endregion
    }
}