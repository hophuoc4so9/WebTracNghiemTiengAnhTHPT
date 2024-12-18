﻿using System.Web.Mvc;

namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult News()
        {
            ViewBag.Message = "Your news page.";

            return View();
        }
        public ActionResult Footer()
        {
            return PartialView("_PartialFooter");
        }
    }
}