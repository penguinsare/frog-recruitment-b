using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class Recruiter
    {
        public int RecruiterId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        [JsonIgnore]
        public List<Client> Clients { get; set; }
        [JsonIgnore]
        public List<Candidate> Candidates { get; set; }

    }
}
