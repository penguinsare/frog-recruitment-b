using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.BindingModels
{
    public class BindCandidate
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public int JobId { get; set; }

        public int CandidateStatusId { get; set; }
        public int RecruiterId { get; set; }
    }
}
