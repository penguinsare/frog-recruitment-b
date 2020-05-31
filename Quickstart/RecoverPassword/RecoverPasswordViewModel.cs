using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost
{
    public class RecoverPasswordViewModel
    {
        [Required]
        public string Username { get; set; }

        [Display(Name = "Recovery key")]
        [Required]
        public string RecoveryKey { get; set; }

        [Display(Name = "New password")]
        [Required]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("NewPassword")]
        [Required]
        public string ConfirmPassword { get; set; }

    }
}
