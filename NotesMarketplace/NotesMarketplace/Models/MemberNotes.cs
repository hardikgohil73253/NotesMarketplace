using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class MemberNotes
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public int TotalDownloads { get; set; }
        public double Earnings { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
    }
}