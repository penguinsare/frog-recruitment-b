using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class CandidateSentToIntrerview
    {
        public int CandidateSentToIntrerviewId { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }

        [JsonIgnore]
        public Job Job { get; set; }

        [JsonIgnore]
        public Candidate Candidate { get; set; }
    }
}
