﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace forumbar.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QuanBartender214Entities : DbContext
    {
        public QuanBartender214Entities()
            : base("name=QuanBartender214Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACTIVITY_LOG> ACTIVITY_LOG { get; set; }
        public virtual DbSet<BAIVIET> BAIVIETs { get; set; }
        public virtual DbSet<BINHLUAN> BINHLUANs { get; set; }
        public virtual DbSet<CHUDE> CHUDEs { get; set; }
        public virtual DbSet<CTDonHang> CTDonHangs { get; set; }
        public virtual DbSet<DANHMUCSP> DANHMUCSPs { get; set; }
        public virtual DbSet<DISCOUNT> DISCOUNTS { get; set; }
        public virtual DbSet<DONHANG> DONHANGs { get; set; }
        public virtual DbSet<GIA> GIAs { get; set; }
        public virtual DbSet<ORDER_HISTORY> ORDER_HISTORY { get; set; }
        public virtual DbSet<PRODUCT_REVIEWS> PRODUCT_REVIEWS { get; set; }
        public virtual DbSet<SANPHAM> SANPHAMs { get; set; }
        public virtual DbSet<USER_INFO> USER_INFO { get; set; }
        public virtual DbSet<USERTYPE> USERTYPEs { get; set; }
        public virtual DbSet<YEUTHICH> YEUTHICHes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}