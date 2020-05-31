using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string ContactPerson { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public string TelephoneOffice { get; set; }
        public string TelephoneMobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public int RecruiterId { get; set; }

        public List<FileRepresentationInDatabase> Documents { get; set; }

        [JsonIgnore]
        public Recruiter Recruiter {get;set;}
       

    }
}
