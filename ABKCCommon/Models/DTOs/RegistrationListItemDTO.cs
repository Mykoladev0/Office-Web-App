using System;

namespace ABKCCommon.Models.DTOs
{
    public class RegistrationListItemDTO
    {
        public int Id { get; set; }
        public DogInfoDTO DogInfo { get; set; }
        public string RegistrationStatus { get; set; }
        public ABKCUserDTO SubmittedBy { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public string RegistrationType { get; set; }

        public bool IsInternational { get; set; }
        public bool OvernightRequested { get; set; }
        public bool RushRequested { get; set; }

        public string RegistrationThumbnailBase64 { get; set; }
    }
}