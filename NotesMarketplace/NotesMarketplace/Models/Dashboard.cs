using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;


namespace NotesMarketplace.Models
{
    public class Dashboard
    {
        public IPagedList<SellerNotes> Progress { get; set; }
        public IPagedList<SellerNotes> Published { get; set; }
    }
}