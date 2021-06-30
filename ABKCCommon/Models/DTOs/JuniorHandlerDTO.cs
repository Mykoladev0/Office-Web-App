using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs
{
    public class JuniorHandlerDTO
    {
        public string Name {get;set;}
        public DateTime Birthdate {get;set;} 
        public int Id {get;set;}
        public string RegisteredOwnerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public DateTime CertificateGenerationDate { get; set; }
    }
}