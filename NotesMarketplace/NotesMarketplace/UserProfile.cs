//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotesMarketplace
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserProfile
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<int> Gender { get; set; }
        public string SecondaryEmailAddress { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Countries Countries { get; set; }
        public virtual ReferenceData ReferenceData { get; set; }
        public virtual Users Users { get; set; }
    }
}
