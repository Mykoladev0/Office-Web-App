using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Key]
        [StringLength(20)]
        public string LoginName { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        [StringLength(1)]
        public string DogsGroup { get; set; }
        [StringLength(1)]
        public string OwnersGroup { get; set; }
        [StringLength(1)]
        public string LittersGroup { get; set; }
        [StringLength(1)]
        public string ShowSetupGroup { get; set; }
        [StringLength(1)]
        public string ShowResultsGroup { get; set; }
        [StringLength(1)]
        public string ClubsGroup { get; set; }
        [StringLength(1)]
        public string ReportsGroup { get; set; }
        [StringLength(1)]
        public string UsersGroup { get; set; }
        [StringLength(1)]
        public string LookupsGroup { get; set; }
        [StringLength(1)]
        public string DefaultsGroup { get; set; }
        public bool? LoggedIn { get; set; }
    }
}
