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
    using System.Collections.Generic;
    
    public partial class tb_warranty
    {
        public int id { get; set; }
        public string warrantyID { get; set; }
        public string itemID { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public int duration { get; set; }
        public string description { get; set; }
        public bool MaChinh { get; set; }
        public Nullable<int> Special { get; set; }
    }
}
