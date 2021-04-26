using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string EmailID { get; set; }
    }
}