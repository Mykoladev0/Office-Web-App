using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models.v2.Registrations
{
    public class RegistrationModel : BaseRegistrationModel<DogRegistrationStatusModel>, IRegistration
    {


        //will handle additional special requests later TB 2.2.19
        //(ABKC Binder and Bully Id request)

        //signatures provided during registration
        public virtual AttachmentModel OwnerSignature { get; set; }
        public virtual AttachmentModel CoOwnerSignature { get; set; }

        //dog info
        public virtual BaseDogModel DogInfo { get; set; }


        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        [NotMapped]
        BaseDogModel IRegistration.PrimaryDogInfo => this.DogInfo;
        /// <summary>
        /// Pedigree FRONT copy that was uploaded with registration
        /// </summary>
        /// <value></value>
        public virtual AttachmentModel FrontPedigree { get; set; }
        /// <summary>
        /// Pedigree BACK (optional) copy that was uploaded with registration
        /// </summary>
        /// <value></value>
        public virtual AttachmentModel BackPedigree { get; set; }
        public virtual AttachmentModel FrontPhoto { get; set; }
        public virtual AttachmentModel SidePhoto { get; set; }

        /// <summary>
        /// base 64 string of front image (48x48)
        /// </summary>
        /// <value></value>
        public string RegistrationThumbnailBase64 { get; set; }

        public override RegistrationTypeEnum RegistrationType => RegistrationTypeEnum.Pedigree;

        public override void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "")
        {
            this.StatusHistory = this.StatusHistory ?? new List<DogRegistrationStatusModel>();
            this.StatusHistory.Add(new DogRegistrationStatusModel
            {
                Status = newStatus,
                StatusChangedBy = setBy,
                Comments = comments
            });

        }
    }
}