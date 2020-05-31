using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.BindingModels
{
    public class BindJob
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public int ClientId { get; set; }
        public int RecruiterId { get; set; }
        public int CandidateId { get; set; }
        public int JobStatusId { get; set; }
        public string Remarks { get; set; }
        public string BaseSalary { get; set; }
        public string Fee { get; set; }
        public string StartDate { get; set; }
    }
}
