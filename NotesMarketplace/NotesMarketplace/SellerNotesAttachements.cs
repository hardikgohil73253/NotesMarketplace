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
    
    public partial class SellerNotesAttachements
    {
        public int ID { get; set; }
        public Nullable<int> NoteID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual SellerNotes SellerNotes { get; set; }
    }
}
