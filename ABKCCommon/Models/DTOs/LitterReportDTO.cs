using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class LitterReportDTO
    {
        public int LitterNumber { get; set; }
        public DateTime ReportGenerationDate { get; set; }
        public string SireName { get; set; }
        public string SireABKCNumber { get; set; }
        public string DamName { get; set; }
        public string DamABKCNumber { get; set; }
        public string Breed { get; set; }
        public DateTime Birthdate { get; set; }
        public ICollection<LitterReportPuppyInformationDTO> PuppiesInformation { get; set; }
    }
}