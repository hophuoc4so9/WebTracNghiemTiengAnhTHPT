﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTracNghiemTiengAnhTHPT.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TracNghiemTiengAnhTHPTEntities1 : DbContext
    {
        public TracNghiemTiengAnhTHPTEntities1()
            : base("name=TracNghiemTiengAnhTHPTEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BaoLoi> BaoLois { get; set; }
        public virtual DbSet<CauHoi> CauHois { get; set; }
        public virtual DbSet<ChiTietKetQua> ChiTietKetQuas { get; set; }
        public virtual DbSet<DangBai> DangBais { get; set; }
        public virtual DbSet<DanhGia> DanhGias { get; set; }
        public virtual DbSet<KetQua> KetQuas { get; set; }
        public virtual DbSet<KyThi> KyThis { get; set; }
        public virtual DbSet<NhomCauHoi> NhomCauHois { get; set; }
        public virtual DbSet<PhongThi> PhongThis { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<LienHe> LienHes { get; set; }
        public virtual DbSet<ViewChitietKyThi_2> ViewChitietKyThi_2 { get; set; }
        public virtual DbSet<LopHoc> LopHocs { get; set; }
        public virtual DbSet<BinhLuan> BinhLuans { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}
