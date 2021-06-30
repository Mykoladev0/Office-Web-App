using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models.v2
{
    public class BaseDogModel : BaseDBModel
    {
        public enum GenderEnum
        {
            Male = 0,
            Female = 1,
            Unknown = 2
        }
        public string DogName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DateOfBirth { get; set; }

        //no more than 50 chars
        public string MicrochipNumber { get; set; }

        public GenderEnum Gender { get; set; }

        public virtual Breeds Breed { get; set; }

        public virtual Colors Color { get; set; }

        //owner
        public virtual Owners Owner { get; set; }
        public virtual Owners CoOwner { get; set; }
        public virtual AttachmentModel OwnerSignature { get; set; }
        public virtual AttachmentModel CoOwnerSignature { get; set; }

        //lineage
        public virtual BaseDogModel Sire { get; set; }
        public virtual BaseDogModel Dam { get; set; }

        public int OriginalDogTableId { get; set; }

        public virtual Litters Litter { get; set; }

        public string ABKCNumber { get; set; }
        public UserModel LastModifiedBy { get; internal set; }
    }
}