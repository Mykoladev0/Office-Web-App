using System;
using ABKCCommon.Models.DTOs;

namespace CoreDAL.Models.DTOs
{
    public class JuniorHandlerRegistrationDTO : BaseDraftRegistrationSubmitDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CertificateNumber { get; set; }

        public string ParentLastName { get; set; }
        public string ParentFirstName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool? International { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }


        public string RegistrationStatus { get; set; }
        public ABKCUserDTO SubmittedBy { get; set; }
    }
}