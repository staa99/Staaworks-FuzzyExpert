//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test.NewFolder1
{
    using System;
    using System.Collections.Generic;
    
    public partial class PersonFinger
    {
        public int PersonFingerId { get; set; }
        public int SnapshotId { get; set; }
        public int FingerId { get; set; }
        public string FID1 { get; set; }
        public string FID2 { get; set; }
        public string FID3 { get; set; }
        public string FID4 { get; set; }
        public string FMD { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> DateLastModified { get; set; }
    
        public virtual IDSnapshot IDSnapshot { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}