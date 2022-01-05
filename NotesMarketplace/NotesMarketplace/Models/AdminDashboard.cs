using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class AdminDashboard
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string FileSize { get; set; }
        public string SellType { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Publisher { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public int TotalDownloads { get; set; }
    }
}