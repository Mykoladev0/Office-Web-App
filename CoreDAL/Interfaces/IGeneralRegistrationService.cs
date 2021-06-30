using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABKCCommon.Models.DTOs;
using CoreDAL.Models;
using CoreDAL.Models.DTOs;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Interfaces
{
    public enum SupportingDocumentTypeEnum
    {
        FrontPedigree = 0,
        BackPedigree = 1,
        FrontPhoto = 2,
        SidePhoto = 3,
        OwnerSignature = 4,
        CoOwnerSignature = 5,
        SellerSignature = 6,
        CoSellerSignature = 7,
        SireOwnerSignature = 8,
        SireCoOwnerSignature = 9,
        DamOwnerSignature = 10,
        DamCoOwnerSignature = 11,
        BillOfSaleFront = 12,
        BillOfSaleBack = 13
    }
    public interface IGeneralRegistrationService
    {

        #region "Submit and Approve"
        /// <summary>
        /// NOTE! Multiple registrations (and types) can tie to one payment, need separate payment table
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        Task<IRegistration> SubmitRegistration(int registrationId, RegistrationTypeEnum registrationType, UserModel savedBy, TransactionModel transaction);
        Task<IRegistration> ApproveRegistration(int id, UserModel user, string comments, RegistrationTypeEnum registrationType);
        Task<bool> RejectRegistration(int registrationId, string reasonForRejection, RegistrationTypeEnum registrationType, UserModel rejectedBy);
        Task<bool> RequestInformation(int registrationId, string infoNeeded, RegistrationTypeEnum registrationType, UserModel user);

        Task<bool> RegistrationValidForSubmission(int id, RegistrationTypeEnum registrationType);
        #endregion

        #region Supporting Documents
        Task<AttachmentModel> GetSupportingDocument(int id, SupportingDocumentTypeEnum documentType, RegistrationTypeEnum regType);
        Task<bool> AddSupportingDocument(int regId, string fileName, byte[] docStream, SupportingDocumentTypeEnum documentType, RegistrationTypeEnum registrationType, UserModel savedBy);
        /// <summary>
        /// returns a collection of document types supplied (and stored) for a given registration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICollection<SupportingDocumentTypeEnum> GetDocumentTypesProvidedForRegistration(int id, RegistrationTypeEnum registrationType);
        #endregion

        #region "Search Methods"

        Task<ICollection<IRegistration>> GetPendingRegistrations();


        Task<ICollection<IRegistration>> GetWaitingInformationRegistrationsForUser(UserModel user);
        Task<ICollection<IRegistration>> GetRejectedRegistrationsForUser(UserModel user);

        Task<ICollection<IRegistration>> GetRegistrationsByRepresentative(int repId);
        Task<ICollection<IRegistration>> GetRegistrationsByOwner(int id);
        Task<ICollection<IRegistration>> SearchRegistrationsByRepresentative(string searchText);
        Task<ICollection<IRegistration>> SearchRegistrationsByOwner(string searchText);
        Task<ICollection<IRegistration>> SearchRegistrationsByDogName(string searchText);

        #endregion

        #region "Payment"

        /// <summary>
        /// finds the cost per registration, subtotal, and any processing fees
        /// These are user dependant
        /// </summary>
        /// <param name="registrationsToSubmit"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<PaymentQuoteDTO> GenerateSubmitQuote(ICollection<RegistrationSubmitDTO> registrationsToSubmit, UserModel user);
        Task<ICollection<IRegistration>> GetPendingRegistrationsForUser(UserModel user);
        Task<ICollection<IRegistration>> GetRegistrationsForUser(UserModel user);

        #endregion
        Task<IRegistration> GetRegistration(int registrationId, RegistrationTypeEnum registrationType);

    }
}