using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Breeds
    {
        [Column("id")]
        public int Id { get; set; }
        [StringLength(30)]
        public string Breed { get; set; }
    }
}
