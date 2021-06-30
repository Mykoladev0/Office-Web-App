using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class LitterReportPuppyInformationDTO
    {
        public string PuppyName { get; set; }
        public string ABKCNumber { get; set; }
        public string Sex { get; set; }
        public string SoldTo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string STZip { get; set; }
    }
}