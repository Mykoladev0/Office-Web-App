namespace CoreDAL.Models.v2.Registrations
{
    public abstract class BaseRegistrationStatusModel : BaseDBModel, IRegistrationStatus
    {
        public RegistrationStatusEnum Status { get; set; }
        public UserModel StatusChangedBy { get; set; }
        //set max length to 150
        public string Comments { get; set; }

        // public abstract BaseRegistrationModel<IRegistrationStatus> Registration { get; set; }

    }

}