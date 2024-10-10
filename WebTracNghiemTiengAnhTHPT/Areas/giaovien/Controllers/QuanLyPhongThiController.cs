using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyPhongThiController : Controller
    {
        // GET: giaovien/QuanLyPhongThi
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<PhongThi> model  = context.PhongThis.ToList();
                return View(model);
            }
        }
    }
}