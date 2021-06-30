using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Transfers
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("Dog_Id")]
        public int? DogId { get; set; }
        [Column("New_Owner_Id")]
        public int NewOwnerId { get; set; }
        [Column("New_CoOwner_Id")]
        public int? NewCoOwnerId { get; set; }
        [Column("Sale_Date", TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }
        [Column("Old_Owner_Id")]
        public int OldOwnerId { get; set; }
        [Column("Old_CoOwner_Id")]
        public int? OldCoOwnerId { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifyDate { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }
}
