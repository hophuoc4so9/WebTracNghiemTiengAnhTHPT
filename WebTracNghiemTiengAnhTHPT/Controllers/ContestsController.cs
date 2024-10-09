using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
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
            List<ViewChitietKyThi_2> model = db.ViewChitietKyThi_2.ToList();
            return View(model);
            
        }
       
        public ActionResult ChiTietKyThi(int made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<CauHoi> model = db.CauHois.ToList();
            model = model.Where(c => c.KyThis.Any(kc => kc.MaDe == made)).ToList();
            ViewBag.MaDe = made;
            if (Session["UserName"] == null)
            {

                return RedirectToAction("Login", "Login", new { area = "admin" });
            }
            else
            return View(model);

        }
        [HttpPost]
        public ActionResult Result(FormCollection form, int made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();

            List<KetQua> results = db.KetQuas.ToList();
            // Duyệt qua từng câu hỏi
            KetQua ketqua = new KetQua();
            ketqua.MaDe = made;
            ketqua.Username = Session["UserName"].ToString();
            db.KetQuas.Add(ketqua);
            db.SaveChanges(); 

            List<CauHoi> cauhois = db.CauHois.Where(c => c.KyThis.Any(k => k.MaDe == made)).ToList();
            foreach (var item in cauhois)
            {
                string questionKey = "answer_" + item.MaCauHoi;
                string selectedValue = form[questionKey];

                ChiTietKetQua tam = new ChiTietKetQua();
                tam.MaCauHoi = item.MaCauHoi;
                tam.DapAnChon = !string.IsNullOrEmpty(selectedValue) ? selectedValue : "N";
                tam.Username = Session["UserName"]?.ToString();
                tam.MaDe = made;
                tam.Maketqua = ketqua.Maketqua; // Now Maketqua should have the correct value
                db.ChiTietKetQuas.Add(tam);
            }
            db.SaveChanges();
            return View(results);
        }




    }
}