using System;
namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class DNACertificateDTO
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public string ABKCNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RegisteredOwnerName { get; set; }
        /// <summary>
        /// House # Street Name Suite # 
        /// </summary>
        /// <value></value>
        public string Address1 { get; set; }
        /// <summary>
        /// Municipality, Province PostalCode Country
        /// </summary>
        /// <value></value>
        public string Address2 { get; set; }

        public string Address3 { get; set; }
        public DateTime DateOfAnalysis {get;set;}
        public DateTime CertificateGenerationDate {get;set;}
    } 
}

