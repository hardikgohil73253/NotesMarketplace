using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class DisplayReview
    {
        public int ReviewID { get; set; }
        public string ReviewBy { get; set; }
        public string UserImage { get; set; }
        public decimal Stars { get; set; }
        public string Comment { get; set; }
    }
}