using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string BaseSalary { get; set; }
        public string Fee { get; set; }
        public string StartDate { get; set; }
        public int? ClientId { get; set; }
        public int RecruiterId { get; set; }
        public List<FileRepresentationInDatabase> Documents { get; set; }
        public string Remarks { get; set; }

        [JsonIgnore]
        public Client Client { get; set; }

        [JsonIgnore]
        public Recruiter Recruiter { get; set; }
        public List<CandidateSentToIntrerview> CandidatesSent { get; set; }
        public Candidate AssignedCandidate { get; set; }
        public JobStatus JobStatus { get; set; }
        
    }
}
