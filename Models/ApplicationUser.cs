using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace crm.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int? RecruiterId { get; set; }
        public Recruiter Recruiter { get; set; }
    }
}