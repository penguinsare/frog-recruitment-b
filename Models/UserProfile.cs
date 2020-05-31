using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Models
{
    public class UserProfile
    {
        public UserProfile(ApplicationUser user)
        {
            UserName = user.UserName;
            RecruiterId = user.Recruiter.RecruiterId;          
        }

        public string UserName { get; set; }
        public int RecruiterId { get; set; }
    }
}
