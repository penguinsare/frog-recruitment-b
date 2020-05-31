using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class JobStatus
    {
        public int JobStatusId { get; set; }

        public string Status { get; set; }
    }
}
