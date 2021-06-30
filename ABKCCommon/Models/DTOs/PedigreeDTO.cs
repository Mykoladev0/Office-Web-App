using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class PedigreeDTO
    {
        #region "Dog Pedigree Information"

        public string Name { get; set; }
        public string Breed { get; set; }
        public string ABKCNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Color { get; set; }
        public string MicrochipNumber { get; set; }
        public int NumberOfPups { get; set; }

        public IEnumerable<string> Certifications { get; set; }
        public string CertificationsString
        {
            get => this.Certifications != null ? string.Join(",", Certifications) : null;
            set => this.Certifications = value != null ? value.Split(',') : null;
        }

        #endregion

        #region "Owner Information"

        /// <summary>
        /// Full Name of Primary Owner
        /// </summary>
        /// <value></value>
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

        #endregion

        #region "Dog Ancestry"

        /// <summary>
        /// First/LastName of Owner
        /// If Co-owner exists, they are concatinated (First Owner AND Co Owner)
        /// </summary>
        /// <value></value>
        public string SireOwnerName { get; set; }

        /// <summary>
        /// First/LastName of Owner
        /// If Co-owner exists, they are concatinated (First Owner AND Co Owner)
        /// </summary>
        /// <value></value>
        public string DamOwnerName { get; set; }

        public PedigreeAncestorDTO Sire { get; set; }
        public PedigreeAncestorDTO Dam { get; set; }

        #endregion

        #region "Pedigree Metadata"

        public DateTime PedigreeGeneratedDate { get; set; }
        public int DogId { get; set; }//will probably use this for a URL endpoint

        #endregion
    }
}