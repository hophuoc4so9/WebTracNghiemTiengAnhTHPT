using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;
namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class ContestsController : Controller
    {
        // GET: Contests
        public ActionResult Index()
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<view_ChitietKyThi> model = db.view_ChitietKyThi.ToList();
            return View(model);
            
        }
        [HttpPost]
        public ActionResult ChiTietKyThi(string made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
        
            List<CauHoi> model = db.CauHois.ToList();
            model = model.Where(c => c.KyThis.Any(kc => kc.MaDe == made)).ToList();
            return View(model);

        }
    }
}