using CoreDAL.Models.v2.Registrations;

namespace CoreDAL.Models.DTOs
{

    /// <summary>
    /// used to identify what registrations are included in the submission package
    /// </summary>
    public class RegistrationSubmitDTO
    {
        public int RegistrationId { get; set; }
        public RegistrationTypeEnum RegistrationType { get; set; }
    }
}