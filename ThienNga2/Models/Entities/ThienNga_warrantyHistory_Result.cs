//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThienNga2.Models.Entities
{
    using System;
    
    public partial class ThienNga_warrantyHistory_Result
    {
        public int id { get; set; }
        public System.DateTime startDate { get; set; }
        public int employee { get; set; }
        public string warrantyID { get; set; }
        public string Description { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> realeaseDATE { get; set; }
        public string itemID { get; set; }
        public Nullable<System.DateTime> finishFixingDate { get; set; }
    }
}
