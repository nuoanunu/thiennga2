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
    
    public partial class log
    {
        public int id { get; set; }
        public string warrantyActivitiesID { get; set; }
        public string action { get; set; }
        public System.DateTime date { get; set; }
        public string account { get; set; }
    
        public virtual account account1 { get; set; }
    }
}