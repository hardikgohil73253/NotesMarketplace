using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;


namespace NotesMarketplace.Models
{
    public class MemberDetails
    {
        public MemberProfile MemberProfile { get; set; }
        public IPagedList<MemberNotes> MemberNotes { get; set; }
    }
}