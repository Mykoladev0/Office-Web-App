using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDAL.Models
{
    public partial class Defaults
    {
        [Column("id")]
        public int Id { get; set; }
        [StringLength(25)]
        public string UserName { get; set; }
        public int? RegFee { get; set; }
        [StringLength(75)]
        public string MergeDataPath { get; set; }
        [StringLength(75)]
        public string ReportsPath { get; set; }
        [Column("LastABKCNo")]
        public int? LastAbkcno { get; set; }
        [Column("Ped_CustomPaperSize")]
        [StringLength(50)]
        public string PedCustomPaperSize { get; set; }
        [Column("PuppyReg_CustomPaperSize")]
        [StringLength(50)]
        public string PuppyRegCustomPaperSize { get; set; }
        [Column("ABKCShowFee")]
        public int? AbkcshowFee { get; set; }
        [Column("NonABKCShowFee")]
        public int? NonAbkcshowFee { get; set; }
        public bool IsLocked { get; set; }
        public int? LastBullyId { get; set; }
        public int? LastOwnerId { get; set; }
        public int? LastLitterId { get; set; }
        public int? LastShowId { get; set; }
        public int? LastClassId { get; set; }
        public int? LastClubId { get; set; }
        public int? LastJrHandlerId { get; set; }
        [StringLength(12)]
        public string CurVersion { get; set; }
    }
}
