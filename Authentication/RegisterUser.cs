using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Authentication
{
    public class RegisterUser
    {
        public string UserName { get; set; }
        public string AccessClaim { get; set; }
        public int RecruiterId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        [JsonRequired]
        public string Name { get; set; }
        public string  Email { get; set; }
        public string Phone { get; set; }
                                           //public class RegisterViewModel
                                           //{
                                           //    //[Required]
                                           //    //[StringLength(50, ErrorMessage = "The {0} must be at least {2}, at max {1} characters long and unique.", MinimumLength = 2)]
                                           //    //[Display(Name = "Username")]


        //    //[Required]
        //    //[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        //    //[Display(Name = "First Name")]
        //    //public string FirstName { get; set; }

        //    //[Required]
        //    //[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        //    //[Display(Name = "Last Name")]
        //    //public string LastName { get; set; }

        //    //[Required]
        //    //[EmailAddress]
        //    //[Display(Name = "Email")]
        //    //public string Email { get; set; }

        //    [Required]
        //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //    [DataType(DataType.Password)]
        //    [Display(Name = "Password")]


        //    [DataType(DataType.Password)]
        //    [Display(Name = "Confirm password")]
        //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

        //}
    }
}
