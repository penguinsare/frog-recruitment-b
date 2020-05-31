using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class CandidateStatus
    {
        public int CandidateStatusId { get; set; }
        public string Status { get; set; }
        public string NormalizedStatus { get; set; }
    }
}
