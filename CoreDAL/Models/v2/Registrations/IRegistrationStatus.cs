using System;

namespace CoreDAL.Models.v2.Registrations
{

    public enum RegistrationStatusEnum
    {
        /// <summary>
        /// Before it has been submitted a registration stays in draft status for client changes
        /// </summary>
        /// <value></value>
        Draft = 0,
        Pending = 1,
        WaitingForDetails = 2,
        Approved = 3,
        Denied = 4,
        Unknown = 5 //ERROR STATE!
    }
    public interface IRegistrationStatus
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        RegistrationStatusEnum Status { get; set; }
        UserModel StatusChangedBy { get; set; }
        //set max length to 150
        string Comments { get; set; }
        // BaseRegistrationModel<IRegistrationStatus> Registration { get; set; }

        // public abstract virtual BaseRegistrationModel<BaseRegistrationStatusModel> Registration { get; set; }

    }
}