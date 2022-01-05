using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class AddType
    {
        public int TypeID { get; set; }
        [Required]
        public string TypeName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}