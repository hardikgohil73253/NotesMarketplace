using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class BuyerRequest
    {
        public int TID { get; set; }
        public int NID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerContact { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<System.DateTime> DownloadedDate { get; set; }
    }
}