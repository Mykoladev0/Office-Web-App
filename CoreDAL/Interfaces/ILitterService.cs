using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs.Pedigree;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    public interface ILitterService
    {
        Task<int> GetLittersCount();
        Task<int> NumberOfPups(int dogId, bool isSire);

        #region "Registration"
        /// <summary>
        /// retrieves the sire and dam by ids and starts a litter registration
        /// </summary>
        /// <param name="sireId">original ID number of sire</param>
        /// <param name="damId">original ID number of dam</param>
        /// <param name="savedBy"></param>
        /// <returns></returns>
        Task<LitterRegistrationModel> StartLitterRegistration(int sireId, int damId, UserModel savedBy);

        /// <summary>
        /// Saves a draft litter that has already been started. Any NON-NULL values in the registration will be used
        /// </summary>
        /// <param name="regId">Litter registration id</param>
        /// <param name="reg">data to update</param>
        /// <param name="user">user posting changes</param>
        /// <returns></returns>
        Task<LitterRegistrationModel> SaveLitterDraft(int regId, LitterDraftDTO reg, UserModel user);

        /// <summary>
        /// retrieves a litter registration by dogId searching new and old tables sires and dams
        /// </summary>
        /// <param name="dogId"></param>
        /// <returns></returns>
        Task<LitterRegistrationModel> GetLitterRegistrationForDog(int dogId);

        /// <summary>
        /// Once a litter has been approved, generate a litter with new puppies in the ABKC system
        /// </summary>
        /// <param name="litterRegistrationId"></param>
        /// <returns></returns>
        Task<Litters> CreateLitterFromRegistration(int litterRegistrationId, UserModel createdBy);

        Task<LitterRegistrationModel> GetLitterRegistrationById(int regId);

        ICollection<SupportingDocumentTypeEnum> GetLitterDocsProvided(int litterRegistrationId);

        /// <summary>
        /// verifies all required litter information is included, throws error if not
        /// will approve and then create a litter in the system from the registration
        /// </summary>
        /// <param name="litterRegistrationId"></param>
        /// <returns></returns>
        Task<LitterRegistrationModel> ApproveLitterRegistration(LitterRegistrationModel litterReg, UserModel approvedBy);
        Task<ICollection<LitterRegistrationModel>> GetAllLitterRegistrations();

        /// <summary>
        /// Finds registration and verifies all information has been entered
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        Task<IRegistration> CanSubmitRegistration(int registrationId);

        /// <summary>
        /// returns all litter registrations that have a status of pending
        /// </summary>
        /// <returns></returns>
        Task<ICollection<LitterRegistrationModel>> GetRegistrationsByStatus(RegistrationStatusEnum? status, UserModel filterByUser = null);
        Task<ICollection<LitterRegistrationModel>> GetLittersByRepresentative(int repId);
        Task<ICollection<LitterRegistrationModel>> GetRegistrationsByOwner(int ownerId);
        Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByRepresentative(string searchText);
        Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByDogName(string searchText);
        Task<ICollection<LitterRegistrationModel>> SearchRegistrationsByOwner(string searchText);

        Task<bool> DeleteRegistration(int id);
        Task<AttachmentModel> GetSupportingDocument(int id, SupportingDocumentTypeEnum documentType);
        /// <summary>
        /// the id of the litter registration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LitterReportDTO> BuildLitterReportFromRegistration(int id);

        #endregion
    }
}
