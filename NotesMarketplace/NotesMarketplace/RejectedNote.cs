//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotesMarketplace
{
    using System;
    using System.Collections.Generic;
    
    public partial class RejectedNote
    {
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int RejectedBy { get; set; }
        public string Comments { get; set; }
    
        public virtual SellerNotes SellerNotes { get; set; }
    }
}
