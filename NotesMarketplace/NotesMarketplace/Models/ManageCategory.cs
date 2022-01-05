using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class ManageCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
    }
}