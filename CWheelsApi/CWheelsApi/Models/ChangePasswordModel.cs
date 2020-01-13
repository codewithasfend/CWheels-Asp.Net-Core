using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CWheelsApi.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        [Compare("NewPassword",ErrorMessage ="The new password and the confirm password does not match")]
        public string ConfirmPassword { get; set; } 
    }
}
