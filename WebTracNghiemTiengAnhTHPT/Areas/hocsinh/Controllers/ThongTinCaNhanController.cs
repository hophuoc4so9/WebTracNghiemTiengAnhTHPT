using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.hocsinh.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1 ();
        // GET: hocsinh/ThongTinCaNhan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ThongTinCaNhan()
        {
            var username = (string)Session["Username"];
            var taiKhoan = db.TaiKhoans.FirstOrDefault(x => x.Username == username);
            return View(taiKhoan);
        }
    }
}