using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class ManualUpdateLog
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("Dog_RecId")]
        public int DogRecId { get; set; }
        [Column("Bully_Id")]
        public int BullyId { get; set; }
        [Required]
        [Column("ABKC_No")]
        [StringLength(12)]
        public string AbkcNo { get; set; }
        [Required]
        [StringLength(50)]
        public string DogName { get; set; }
        [StringLength(25)]
        public string Title { get; set; }
        public int? OldPoints { get; set; }
        public int? OldChampWins { get; set; }
        public int? NewPoints { get; set; }
        public int? NewChampWins { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateInserted { get; set; }
        [StringLength(50)]
        public string InsertedBy { get; set; }
    }
}
