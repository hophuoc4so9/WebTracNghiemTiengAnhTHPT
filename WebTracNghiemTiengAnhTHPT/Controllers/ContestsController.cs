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
            KyThi kt = db.KyThis.SingleOrDefault(k => k.MaDe == made);
            List<CauHoi> model = kt.CauHois.ToList();
            ViewBag.MaDe = made;
            if (Session["UserName"] == null)
            {

                return RedirectToAction("Login", "Login", new { area = "admin" });
            }
            else
            {
                Session["time" + made] = DateTime.Now.AddMinutes(kt.ThoiGian);
                return View(model);
            }    
           

        }
        [HttpPost]
        public ActionResult Result(FormCollection form, int made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            KetQua ketqua = new KetQua();
            ketqua.MaDe = made;
            ketqua.Username = Session["UserName"].ToString();
            db.KetQuas.Add(ketqua);
            db.SaveChanges();
                        List<CauHoi> cauhois = db.CauHois.Where(c => c.KyThis.Any(k => k.MaDe == made)).ToList();
            int total=cauhois.Count;
            int correct = 0;
            foreach (var item in cauhois)
            {
                string questionKey = "answer_" + item.MaCauHoi;
                string selectedValue = form[questionKey];

                ChiTietKetQua tam = new ChiTietKetQua();
                tam.MaCauHoi = item.MaCauHoi;
                tam.DapAnChon = !string.IsNullOrEmpty(selectedValue) ? selectedValue : "N";
                tam.Username = Session["UserName"]?.ToString();
                
                tam.MaDe = made;
                tam.Maketqua = ketqua.Maketqua; 
                if(item.DapAnChinhXac.ToLower().Contains(tam.DapAnChon.ToLower()))
                {
                    correct++;
                }
              
                db.ChiTietKetQuas.Add(tam);
            }
            ketqua.Diem = correct * 10 / total;
            db.SaveChanges();

            return RedirectToAction("Result", new { maketqua = ketqua.Maketqua });  
        }

        [HttpGet]
        public ActionResult Result(int maketqua)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            KetQua kq= db.KetQuas.SingleOrDefault(k => k.Maketqua == maketqua);

            return View(kq);
        }
        [HttpGet]
        public ActionResult ChiTiet(int made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();

            KyThi ky =  db.KyThis.SingleOrDefault(k => k.MaDe == made);
            ViewBag.MaDe = made;

            return View(ky);
        }
        public ActionResult PartialDanhGiaKyThi(int  made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<DanhGia> model = db.DanhGias.Where(c => c.MaDe == made).ToList();
            return View(model);
        }
        public ActionResult PartialLichSuLamBai(int made)
        {
         
                string username = Session["UserName"] == null ? "": Session["UserName"].ToString() ;
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<KetQua> model = db.KetQuas.Where(c => c.MaDe == made && c.Username == username).ToList();
            return View(model);
        }




    }
}