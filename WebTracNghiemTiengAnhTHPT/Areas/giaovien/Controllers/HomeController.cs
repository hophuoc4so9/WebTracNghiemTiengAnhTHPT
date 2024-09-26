using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class HomeController : Controller
    {
        // GET: giaovien/Home
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult RenderNavbar()
        {
            return PartialView("_Navbar");
        }
        public ActionResult PartialPhanQuyen()
        {
            if (Session["phanquyen"] == null)
            {
                return null;
            }
            if (Session["phanquyen"].ToString() == "admin")
            {
                return PartialView("_PartialChucNangAdmin");
            }
            else
            {
                return PartialView("_PartialChucNangGiaoVien");
            }
        }
    }
}