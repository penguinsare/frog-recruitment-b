using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Authentication
{
    public class OutputUser
    {
        public string UserName { get; set; }
        public string AccessPermission { get; set; }
        public OutputRecruiter Recruiter { get; set; }
        public bool CanRemoveUser { get; set; }
    }
}
