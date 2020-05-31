using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Authentication
{
    public class UpdateUser
    {
        public string UserName { get; set; }
        public string Role { get; set; }
        public int RecruiterId { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
