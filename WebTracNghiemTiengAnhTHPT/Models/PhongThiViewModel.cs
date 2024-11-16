using System.Collections.Generic;
using System.Web.Mvc;

namespace WebTracNghiemTiengAnhTHPT.Models
{
    public class PhongThiViewModel
    {
        public PhongThi PhongThi { get; set; }
        public IEnumerable<SelectListItem> LopHocList { get; set; }
    }
}