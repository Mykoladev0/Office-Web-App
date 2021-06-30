using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class PedigreeAncestorDTO
    {
        public string Name { get; set; }
        public string ABKCNumber { get; set; }
        public string Color { get; set; }
        public int NumberOfPups { get; set; }

        /// <summary>
        /// will be a list of titles/certs, and degrees
        /// See Pedigree for descriptions and abbreviations
        /// List will need to be concatinated with ',' for printing
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Certifications { get; set; }
        public string CertificationsString
        {
            get => this.Certifications != null ? string.Join(",", Certifications) : null;
            set => this.Certifications = value != null ? value.Split(',') : null;
        }

        public PedigreeAncestorDTO Sire { get; set; }
        public PedigreeAncestorDTO Dam { get; set; }
    }
}