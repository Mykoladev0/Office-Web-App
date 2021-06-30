using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class UserLoginHistory
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string LoginName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LoggedIn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LoggedOut { get; set; }
        [StringLength(25)]
        public string Version { get; set; }
    }
}
