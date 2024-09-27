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