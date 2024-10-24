using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class PhongThiController : Controller
    {
        // GET: PhongThi
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
        public ActionResult Index()
        {
            string username = Session["UserName"]?.ToString() ?? "";
            List<PhongThi> otherlist = db.PhongThis
     .Where(n => !n.TaiKhoans.Any(a => a.Username == username))
     .ToList();

            List<PhongThi> mylist = db.PhongThis
                .Where(n => n.TaiKhoans.Any(a => a.Username == username))
                .ToList();

            ViewBag.MyPhongThis = mylist;
            ViewBag.OtherPhongThis = otherlist;

            return View();
        }
        [HttpPost]
     
        public JsonResult JoinPhongThi(int PhongThiId, string password)
        {
            var phongThi = db.PhongThis.Find(PhongThiId);
          

            if (phongThi.MatKhau != password)
            {
                return Json(new { success = false, message = "Incorrect password." });
            }

            return Json(new { success = true, message = "You have successfully joined the PhongThi." });
        }

    }
}