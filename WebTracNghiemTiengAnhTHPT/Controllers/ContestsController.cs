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
            TracNghiemTiengAnhTHPTEntities db = new TracNghiemTiengAnhTHPTEntities(); 
            List<KyThi> model= db.KyThis.ToList();
            return View(model);
            
        }
    }
}