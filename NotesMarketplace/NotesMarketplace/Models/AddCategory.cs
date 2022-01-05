using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketplace.Models
{
    public class AddCategory
    {
        public int CategoryID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}