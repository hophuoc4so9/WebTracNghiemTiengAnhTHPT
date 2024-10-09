using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyCauHoiController : Controller
    {
        // GET: giaovien/QuanLyCauHoi
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Get the current exam's questions
                var model = context.CauHois
            .Include("NhomCauHoi")// Include related data if needed
            .ToList();
                return View(model);
            }
        }
        [HttpPost]
        public JsonResult DeleteQuestion(int maCauHoi)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                try
                {
                    var question = db.CauHois.FirstOrDefault(q => q.MaCauHoi == maCauHoi);
                    if (question != null)
                    {
                        question.isDeleted = true;
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }    
                
        }
    }
}