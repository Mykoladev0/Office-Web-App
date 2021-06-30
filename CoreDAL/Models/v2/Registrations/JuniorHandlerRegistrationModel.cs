using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models.v2.Registrations
{
    public class JuniorHandlerRegistrationModel : BaseRegistrationModel<JuniorHandlerRegistrationStatusModel>, IRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
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

        public override RegistrationTypeEnum RegistrationType => RegistrationTypeEnum.JuniorHandler;

        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        [NotMapped]
        BaseDogModel IRegistration.PrimaryDogInfo => new BaseDogModel
        {
            DogName = $"{FirstName} {LastName}",
            DateOfBirth = DateOfBirth.HasValue ? DateOfBirth.Value : DateTime.MinValue,
        };

        public override void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "")
        {
            this.StatusHistory.Add(new JuniorHandlerRegistrationStatusModel
            {
                Status = newStatus,
                StatusChangedBy = setBy,
                Comments = comments
            });
        }

        [NotMapped]
        public string RegistrationThumbnailBase64 { get { return string.Empty; } set { } }
    }
}