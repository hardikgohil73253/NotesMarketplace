using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,}", ErrorMessage = "Invalid Password format.")]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}