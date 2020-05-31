using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.BindingModels
{
    public class BindChangePassword
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
