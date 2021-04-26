using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class AddNotes
    {
        public int? ID { get; set; }
        public int UID { get; set; }
        [Required]
        public string Title { get; set; }
        
        public int CategoryID { get; set; }
        
        public HttpPostedFileBase DisplayPicture { get; set; }
        public HttpPostedFileBase[] File { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<int> NumberOfPages { get; set; }
        [Required]
        public string Description { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> Price { get; set; }
        public HttpPostedFileBase PreviewAttachment { get; set; }
        public string Status { get; set; }
        public int ActionBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}