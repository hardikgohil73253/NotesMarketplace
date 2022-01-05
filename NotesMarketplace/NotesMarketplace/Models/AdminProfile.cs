using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class AdminProfile
    {
        public int UserID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [EmailAddress]
        public string SecondaryEmail { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}