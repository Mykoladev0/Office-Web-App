using System;
using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs.Pedigree
{
    public class SaveABullyDTO
    {
        public string RescueDog {get;set;}
        public string Breed {get;set;}
        public string Gender {get;set;}
        public DateTime Birthday {get;set;}
        public DateTime RescueDate {get;set;}
        public string Color {get;set;}
        public string OwnedBy {get;set;}
        public string RegisteredOwnerName {get;set;}
        public string Address1 {get;set;}
        public string Address2 {get;set;}
        public string Address3 {get;set;}
        public DateTime CertificateGenerationDate {get;set;}
    }
}