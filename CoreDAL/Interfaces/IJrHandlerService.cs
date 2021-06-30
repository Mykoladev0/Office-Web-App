using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    public interface IJrHandlerService
    {
        #region Junior Handlers
        Task<JuniorHandlerRegistrationModel> StartJrHandlerRegistration(JuniorHandlerRegistrationDTO reg, UserModel savedBy);
        Task<JuniorHandlerRegistrationModel> SaveDraftJrRegistration(JuniorHandlerRegistrationDTO registration, UserModel savedBy);
        Task<JuniorHandlerRegistrationModel> GetJrRegistrationById(int id);
        Task<ICollection<JuniorHandlerRegistrationModel>> GetAllJrRegistrations();

        ICollection<SupportingDocumentTypeEnum> GetJrHandlerDocsProvided(int regId);

        /// <summary>
        /// verifies all required jr handler information is included, throws error if not
        /// will approve and then create a jr handler in the system from the registration
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="approvedBy"></param>
        /// <returns></returns>
        Task<JuniorHandlerRegistrationModel> ApproveRegistration(JuniorHandlerRegistrationModel reg, UserModel approvedBy);

        /// <summary>
        /// Finds registration and verifies all information has been entered
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        Task<IRegistration> CanSubmitRegistration(int registrationId);

        /// <summary>
        /// returns all registrations that have a status of pending
        /// </summary>
        /// <returns></returns>
        Task<ICollection<JuniorHandlerRegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null);
        Task<ICollection<JuniorHandlerRegistrationModel>> GetByRepresentative(int repId);
        Task<ICollection<JuniorHandlerRegistrationModel>> GetRegistrationsByOwner(int ownerId);
        Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByRepresentative(string searchText);
        Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByDogName(string searchText);
        Task<ICollection<JuniorHandlerRegistrationModel>> SearchRegistrationsByOwner(string searchText);
        Task<bool> DeleteRegistration(int id);
        Task<JuniorHandlerRegistrationModel> StartDraftRegistration(JuniorHandlerRegistrationDTO registration, UserModel savedBy);

        #endregion
    }
}