//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class DanhGia
    {
        public string Username { get; set; }
        public int Rate { get; set; }
        public string NoiDung { get; set; }
        public string MaDe { get; set; }
    
        public virtual TaiKhoan TaiKhoan { get; set; }
        public virtual KyThi KyThi { get; set; }
    }
}