using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketplace.Models
{
    public class AddCountry
    {
        public int CountryID { get; set; }
        [Required]
        public string CountryName { get; set; }
        [Required]
        public string CountryCode { get; set; }
    }
}