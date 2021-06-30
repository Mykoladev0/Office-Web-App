using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Colors
    {
        public int Id { get; set; }
        [StringLength(25)]
        public string Color { get; set; }
    }
}
