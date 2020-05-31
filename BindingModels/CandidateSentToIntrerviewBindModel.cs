using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.BindingModels
{
    public class CandidateSentToIntrerviewBindModel
    {
        public int CandidateId { get; set; }
        public int JobId { get; set; }
        public int RecruiterId { get; set; }
    }
}
