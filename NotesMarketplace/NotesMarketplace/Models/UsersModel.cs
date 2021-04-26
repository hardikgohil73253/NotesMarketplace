using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class UsersModel
    {
        public int ID { get; set; }
        public int RoleID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailID { get; set; }

        [Required]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,}", ErrorMessage = "Invalid Password format.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Incorrect Password")]
        public string ConfirmPassword { get; set; }
        public bool IsEmailVerified { get; set; }
        public Nullable<System.DateTime> CreatedData { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.Guid> SecretCode { get; set; }



    }
}