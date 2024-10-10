using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyLopHocController : Controller
    {
        // GET: giaovien/QuanLyLopHoc
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<LopHoc> model = context.LopHocs.ToList();
                return View(model);
            }
        }
      
    }
}