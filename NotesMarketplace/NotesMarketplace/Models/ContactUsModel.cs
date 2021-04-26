using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class ContactUsModel
    {
        public int ID { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailID { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Comments { get; set; }
    }
}