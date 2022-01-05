using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class Members
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public int? UnderReviewNotes { get; set; }
        public int? PublishedNotes { get; set; }
        public int? DownloadedNotes { get; set; }
        public int? TotalExpenses { get; set; }
        public int? TotalEarnings { get; set; }
        public bool IsActive { get; set; }
    }
}