using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoreDAL.Models.v2.Registrations
{
    public class PuppyRegistrationModel : BaseRegistrationModel<PuppyRegistrationStatusModel>, IRegistration
    {
        public PuppyRegistrationModel()
        {
            IsTransferRequest = false;
        }

        //dog info
        // [ForeignKey("DogId")]
        public virtual BaseDogModel DogInfo { get; set; }
        /// <summary>
        /// may be null, depending on type of registration (only permanent, pedigree, litter, bullyid, and transfer supported)
        /// </summary>
        /// <value></value>
        [NotMapped]
        BaseDogModel IRegistration.PrimaryDogInfo => this.DogInfo;

        public virtual Owners NewOwner { get; set; }
        public virtual Owners NewCoOwner { get; set; }

        public DateTime? DateOfSale { get; set; }

        //next 4 only used for puppy registration
        public AttachmentModel OwnerSignature { get; set; }
        public AttachmentModel CoOwnerSignature { get; set; }
        public AttachmentModel SellerSignature { get; set; }
        public AttachmentModel CoSellerSignature { get; set; }

        //following 2 are only used for transfers
        public AttachmentModel BillOfSaleFront { get; set; }
        public AttachmentModel BillOfSaleBack { get; set; }

        //for tracking purposes
        public Transfers TransferCreatedFromRegistration { get; set; }
        [NotMapped]
        public string RegistrationThumbnailBase64 { get { return string.Empty; } set { } }
        public override RegistrationTypeEnum RegistrationType => IsTransferRequest ? RegistrationTypeEnum.Transfer : RegistrationTypeEnum.Puppy;

        /// <summary>
        /// Puppy registrations can be used for transfers
        /// only difference is the name won't be changed
        /// </summary>
        /// <value></value>
        public bool IsTransferRequest { get; set; }

        public override void SetStatus(RegistrationStatusEnum newStatus, UserModel setBy, string comments = "")
        {
            this.StatusHistory.Add(new PuppyRegistrationStatusModel
            {
                Status = newStatus,
                StatusChangedBy = setBy,
                Comments = comments
            });
        }
    }
}