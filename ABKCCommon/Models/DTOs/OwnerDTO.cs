using System;
using System.Collections.Generic;
using System.Linq;

namespace ABKCCommon.Models.DTOs
{
    public class OwnerDTO
    {
        public string FullName
        {
            get
            {
                List<string> names = new List<string> { FirstName ?? "", LastName ?? "" };
                string fullName = String.Join(" ", names.Where(n => !String.IsNullOrEmpty(n)));
                return fullName;
            }
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool International { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}