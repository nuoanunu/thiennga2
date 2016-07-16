﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ThienNgaDatabaseEntities : DbContext
    {
        public ThienNgaDatabaseEntities()
            : base("name=ThienNgaDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<account> accounts { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<employee> employees { get; set; }
        public virtual DbSet<inventory> inventories { get; set; }
        public virtual DbSet<item> items { get; set; }
        public virtual DbSet<log> logs { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<tb_cate> tb_cate { get; set; }
        public virtual DbSet<tb_customer> tb_customer { get; set; }
        public virtual DbSet<tb_inventory_name> tb_inventory_name { get; set; }
        public virtual DbSet<tb_product_detail> tb_product_detail { get; set; }
        public virtual DbSet<tb_warranty> tb_warranty { get; set; }
        public virtual DbSet<tb_warranty_activities> tb_warranty_activities { get; set; }
        public virtual DbSet<tb_warrnaty_status> tb_warrnaty_status { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
    
        public virtual ObjectResult<ThienNga_checkkho_Result> ThienNga_checkkho(string productcode)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_checkkho_Result>("ThienNga_checkkho", productcodeParameter);
        }
    
        public virtual ObjectResult<ThienNga_findbyIMEI_Result> ThienNga_findbyIMEI(string iMEI)
        {
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_findbyIMEI_Result>("ThienNga_findbyIMEI", iMEIParameter);
        }
    
        public virtual ObjectResult<ThienNga_FindProduct_Result> ThienNga_FindProduct(string productcode)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_FindProduct_Result>("ThienNga_FindProduct", productcodeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> ThienNga_FindProductDetailID(string productcode)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("ThienNga_FindProductDetailID", productcodeParameter);
        }
    
        public virtual ObjectResult<ThienNga_findwarranty_Result> ThienNga_findwarranty(string warrantycode)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_findwarranty_Result>("ThienNga_findwarranty", warrantycodeParameter);
        }
    
        public virtual ObjectResult<ThienNga_findwarrantyByIMEI_Result> ThienNga_findwarrantyByIMEI(string imei)
        {
            var imeiParameter = imei != null ?
                new ObjectParameter("imei", imei) :
                new ObjectParameter("imei", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_findwarrantyByIMEI_Result>("ThienNga_findwarrantyByIMEI", imeiParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> ThienNga_Login(string usernamee, string passwordd)
        {
            var usernameeParameter = usernamee != null ?
                new ObjectParameter("usernamee", usernamee) :
                new ObjectParameter("usernamee", typeof(string));
    
            var passworddParameter = passwordd != null ?
                new ObjectParameter("passwordd", passwordd) :
                new ObjectParameter("passwordd", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("ThienNga_Login", usernameeParameter, passworddParameter);
        }
    
        public virtual ObjectResult<ThienNga_TimSDT_Result> ThienNga_TimSDT(string phone)
        {
            var phoneParameter = phone != null ?
                new ObjectParameter("phone", phone) :
                new ObjectParameter("phone", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_TimSDT_Result>("ThienNga_TimSDT", phoneParameter);
        }
    
        public virtual ObjectResult<ThienNga_warrantyHistory_Result> ThienNga_warrantyHistory(string warrantycode)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ThienNga_warrantyHistory_Result>("ThienNga_warrantyHistory", warrantycodeParameter);
        }
    
        public virtual ObjectResult<inventory> ThienNga_checkkho2(string productcode)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<inventory>("ThienNga_checkkho2", productcodeParameter);
        }
    
        public virtual ObjectResult<inventory> ThienNga_checkkho2(string productcode, MergeOption mergeOption)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<inventory>("ThienNga_checkkho2", mergeOption, productcodeParameter);
        }
    
        public virtual ObjectResult<tb_product_detail> ThienNga_FindProduct2(string productcode)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_product_detail>("ThienNga_FindProduct2", productcodeParameter);
        }
    
        public virtual ObjectResult<tb_product_detail> ThienNga_FindProduct2(string productcode, MergeOption mergeOption)
        {
            var productcodeParameter = productcode != null ?
                new ObjectParameter("productcode", productcode) :
                new ObjectParameter("productcode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_product_detail>("ThienNga_FindProduct2", mergeOption, productcodeParameter);
        }
    
        public virtual ObjectResult<tb_warranty> ThienNga_findwarranty2(string warrantycode)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty>("ThienNga_findwarranty2", warrantycodeParameter);
        }
    
        public virtual ObjectResult<tb_warranty> ThienNga_findwarranty2(string warrantycode, MergeOption mergeOption)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty>("ThienNga_findwarranty2", mergeOption, warrantycodeParameter);
        }
    
        public virtual ObjectResult<tb_customer> ThienNga_TimSDT2(string phone)
        {
            var phoneParameter = phone != null ?
                new ObjectParameter("phone", phone) :
                new ObjectParameter("phone", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_customer>("ThienNga_TimSDT2", phoneParameter);
        }
    
        public virtual ObjectResult<tb_customer> ThienNga_TimSDT2(string phone, MergeOption mergeOption)
        {
            var phoneParameter = phone != null ?
                new ObjectParameter("phone", phone) :
                new ObjectParameter("phone", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_customer>("ThienNga_TimSDT2", mergeOption, phoneParameter);
        }
    
        public virtual ObjectResult<tb_warranty_activities> ThienNga_warrantyHistory2(string warrantycode)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty_activities>("ThienNga_warrantyHistory2", warrantycodeParameter);
        }
    
        public virtual ObjectResult<tb_warranty_activities> ThienNga_warrantyHistory2(string warrantycode, MergeOption mergeOption)
        {
            var warrantycodeParameter = warrantycode != null ?
                new ObjectParameter("warrantycode", warrantycode) :
                new ObjectParameter("warrantycode", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty_activities>("ThienNga_warrantyHistory2", mergeOption, warrantycodeParameter);
        }
    
        public virtual ObjectResult<tb_warranty> ThienNga_findwarrantyByIMEI2(string imei)
        {
            var imeiParameter = imei != null ?
                new ObjectParameter("imei", imei) :
                new ObjectParameter("imei", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty>("ThienNga_findwarrantyByIMEI2", imeiParameter);
        }
    
        public virtual ObjectResult<tb_warranty> ThienNga_findwarrantyByIMEI2(string imei, MergeOption mergeOption)
        {
            var imeiParameter = imei != null ?
                new ObjectParameter("imei", imei) :
                new ObjectParameter("imei", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<tb_warranty>("ThienNga_findwarrantyByIMEI2", mergeOption, imeiParameter);
        }
    
        public virtual ObjectResult<item> ThienNga_findbyIMEI2(string iMEI)
        {
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<item>("ThienNga_findbyIMEI2", iMEIParameter);
        }
    
        public virtual ObjectResult<item> ThienNga_findbyIMEI2(string iMEI, MergeOption mergeOption)
        {
            var iMEIParameter = iMEI != null ?
                new ObjectParameter("IMEI", iMEI) :
                new ObjectParameter("IMEI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<item>("ThienNga_findbyIMEI2", mergeOption, iMEIParameter);
        }
    }
}
