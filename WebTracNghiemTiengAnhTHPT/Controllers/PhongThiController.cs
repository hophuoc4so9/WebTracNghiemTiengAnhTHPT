using System.Collections.Generic;
using System.Linq;
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
            string username = Session["UserName"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(Session["UserName"]?.ToString()))
            {
                return Json(new { success = false, message = "Bạn phải đăng nhập để vào PhongThi." });
            }
            var phongThi = db.PhongThis.Find(PhongThiId);


            if (phongThi.MatKhau != password)
            {

                return Json(new { success = false, message = "Sai password." });
            }
            phongThi.TaiKhoans.Add(db.TaiKhoans.Find(username));
            db.SaveChanges();
            return Json(new { success = true, message = "You have successfully joined the PhongThi." });
        }
        public ActionResult ChiTietPhongThi(int PhongThiId)
        {
            return RedirectToAction("Index", "Contests", new { PhongThiId = PhongThiId });

        }

    }
}