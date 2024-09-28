using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QL_LOPHOC()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ChangeStatus(string Username, bool isActive)
        {
            try
            {
                var gg = Username; 
                TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
                List<TaiKhoan> model = db.TaiKhoans.ToList();
                // Assuming you have a DbContext for accessing the database
                var user = model.SingleOrDefault(u => u.Username == gg);
                if (user != null)
                {
                    user.status = isActive;  
                    db.SaveChanges(); 
                }
                TempData["SuccessMessage"] = "Bạn đã thay đổi trạng thái thành công.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Render_QLLOPHOC()
        {

            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<TaiKhoan> model = db.TaiKhoans.ToList();
            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult RenderNavbar()
        {
            return PartialView("_Navbar");
        }
        public ActionResult RenderMain()
        {

            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<TaiKhoan> model = db.TaiKhoans.ToList();
            return PartialView(model);
        }

        public ActionResult PartialPhanQuyen()
        {
            if(Session["phanquyen"]==null)
            {
                return null;
            }    

            if (Session["phanquyen"].ToString() == "admin")
             {
                return PartialView("_PartialChucNangAdmin");
            }
            else
            {
                return PartialView("~/Areas/giaovien/Views/Shared/_PartialChucNangGiaoVien.cshtml");
            }    
        }
    }
}