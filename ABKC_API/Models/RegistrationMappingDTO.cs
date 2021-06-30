using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    public class RegistrationMappingDTO
    {
        public int RegistrationId { get; set; }
        public RegistrationTypeEnum RegistrationType { get; set; }
    }
}