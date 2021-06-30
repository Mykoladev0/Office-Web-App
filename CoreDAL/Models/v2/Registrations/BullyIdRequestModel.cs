using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models.v2.Registrations
{
    public class BullyIdRequestModel : BaseRegistrationModel<BullyIdRequestStatusModel>, IRegistration
    {
        public BullyIdRequestModel()
        {
        }
        public virtual BaseDogModel DogInfo { get; set; }


        public AttachmentModel FrontPhoto { get; set; }

        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        [NotMapped]
        BaseDogModel IRegistration.PrimaryDogInfo => this.DogInfo;


        [NotMapped]
        public string RegistrationThumbnailBase64 { get { return string.Empty; } set { } }
        public override RegistrationTypeEnum RegistrationType => RegistrationTypeEnum.BullyId;


        public override void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "")
        {
            this.StatusHistory.Add(new BullyIdRequestStatusModel
            {
                Status = newStatus,
                StatusChangedBy = setBy,
                Comments = comments
            });
        }
    }
}