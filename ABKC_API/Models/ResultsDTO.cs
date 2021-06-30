using CoreDAL.Models.v2.Registrations;

namespace CoreApp.Models
{
    public class RegistrationResultDTO
    {
        public int RegistrationId { get; set; }
        public RegistrationTypeEnum RegistrationType { get; set; }
        public bool Successful { get; set; }
        public string Reason { get; set; }
    }
}