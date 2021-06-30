using System;
using System.Collections.Generic;

namespace CoreDAL.Models.v2.Registrations
{
    public interface IRegistration
    {
        int Id { get; set; }
        bool IsInternationalRegistration { get; set; }
        bool RushRequested { get; set; }

        //can only be true if !IsInternationalRegistration
        bool OvernightRequested { get; set; }

        UserModel SubmittedBy { get; set; }
        DateTime? DateSubmitted { get; }
        RegistrationStatusEnum CurStatus { get; }
        RegistrationTypeEnum RegistrationType { get; }

        string RegistrationThumbnailBase64 { get; set; }

        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        BaseDogModel PrimaryDogInfo { get; }

        //swipe CC info and amount charged
        TransactionModel AssociatedTransaction { get; set; }
        //store tracking number if approved

        string SubmissionNotes { get; set; }

        /// <summary>
        /// updates the current status of the registration
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="setBy"></param>
        void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "");
    }
}