using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models.v2.Registrations
{
    public class LitterRegistrationModel : BaseRegistrationModel<LitterRegistrationStatusModel>, IRegistration
    {
        public virtual Breeds Breed { get; set; }

        public virtual BaseDogModel Sire { get; set; }
        public virtual BaseDogModel Dam { get; set; }
        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        [NotMapped]
        BaseDogModel IRegistration.PrimaryDogInfo => this.Dam ?? this.Sire;

        public DateTime? DateOfBreeding { get; set; }
        public DateTime? DateOfLitterBirth { get; set; }
        public int NumberOfMalesBeingRegistered { get; set; }
        public int NumberOfFemalesBeingRegistered { get; set; }
        public bool FrozenSemenUsed { get; set; }
        public DateTime? DateSemenCollected { get; set; }

        public AttachmentModel SireOwnerSignature { get; set; }
        public AttachmentModel SireCoOwnerSignature { get; set; }
        public AttachmentModel DamOwnerSignature { get; set; }
        public AttachmentModel DamCoOwnerSignature { get; set; }

        public override RegistrationTypeEnum RegistrationType => RegistrationTypeEnum.Litter;
        public override void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "")
        {
            this.StatusHistory.Add(new LitterRegistrationStatusModel
            {
                Status = newStatus,
                StatusChangedBy = setBy,
                Comments = comments
            });
        }
        [NotMapped]
        public string RegistrationThumbnailBase64 { get { return string.Empty; } set { } }

        public Litters LitterFromRegistration { get; set; }
    }
}