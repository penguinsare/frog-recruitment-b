using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class Candidate
    {
        public int CandidateId {get;set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<FileRepresentationInDatabase> Documents { get; set; }
        public int? JobId { get; set; }
        public int RecruiterId { get; set; }
        public bool InterviewsForJob { get; set; }
        public List<CandidateSentToIntrerview> AppliedForJobs { get; set; }

        public CandidateStatus CandidateStatus { get; set; }

        [JsonIgnore]
        public Job Job{ get; set; }

        [JsonIgnore]
        public Recruiter Recruiter { get; set; }

    }
}
