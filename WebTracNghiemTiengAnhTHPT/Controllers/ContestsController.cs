using DocumentFormat.OpenXml.Drawing.Charts;
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
           
            if (Session["UserName"] == null)
            {

                return RedirectToAction("Login", "Login", new { area = "admin" });
            }
            else
            {
                
                TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
                string username = Session["UserName"].ToString();
                KetQua ketQua=db.KetQuas.FirstOrDefault(k =>  k.Username == username
                    && k.status==false && k.thoigian_ketthuc>DateTime.Now);
                if (ketQua != null)
                {
               if(ketQua.MaDe!=made)     ViewBag.Message = "Bạn đang ở trong một kỳ thi khác nên không thể tham gia.";
                    ViewBag.endTime = ketQua.thoigian_ketthuc;
                    return RedirectToAction("BaiLam", new { id = ketQua.Maketqua });
                }
                 
                KyThi kt = db.KyThis.SingleOrDefault(k => k.MaDe == made);
                if(!(kt.SoCauHoi.HasValue && kt.SoCauHoi > 0))
                {
                    kt.SoCauHoi = kt.CauHois.Count();
                }
                ketQua = new KetQua();
                ketQua.Username = username;
                ketQua.MaDe = made;
               
                ketQua.status = false;
                db.KetQuas.Add(ketQua);
                // Shuffle the list of questions
                Random rng = new Random();
                List<CauHoi> shuffledQuestions = kt.CauHois.OrderBy(q => rng.Next()).ToList();

                // Take the first kt.SoCauHoi questions
                List<CauHoi> selectedQuestions = shuffledQuestions.Take(kt.SoCauHoi ?? 0).ToList();
                db.SaveChanges();

                // Create ChiTietKetQua entries with the answer set to "N"
                foreach (var question in selectedQuestions)
                {
                    ChiTietKetQua chiTietKetQua = new ChiTietKetQua
                    {
                        MaCauHoi = question.MaCauHoi,
                        DapAnChon = "N",
                        Username = Session["UserName"]?.ToString(),
                        MaDe = made,
                        Maketqua = ketQua.Maketqua
                    };
                    ketQua.ChiTietKetQuas.Add(chiTietKetQua);
                }

                ketQua.thoigian_batdau=DateTime.Now;
                ketQua.thoigian_ketthuc=DateTime.Now.AddMinutes(kt.ThoiGian );
                db.SaveChanges();
          
                return RedirectToAction("BaiLam", new { id = ketQua.Maketqua });
            }    
           

        }
        public ActionResult BaiLam(int id)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            KetQua kq = db.KetQuas.SingleOrDefault(k => k.Maketqua == id);
            ViewBag.MaDe = kq.Maketqua;
            ViewBag.endTime = kq.thoigian_ketthuc;
            return View(kq);
        }
        [HttpPost]
        public ActionResult Result(FormCollection form, int made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            KetQua ketqua = db.KetQuas.Where(n=>n.Maketqua== made).FirstOrDefault();   
            ketqua.status = true;
         
            int total=ketqua.ChiTietKetQuas.Count;
            double correct = 0;
            foreach (var item in ketqua.ChiTietKetQuas)
            {
                string questionKey = "answer_" + item.MaCauHoi;
                string selectedValue = form[questionKey];
                item.DapAnChon = !string.IsNullOrEmpty(selectedValue) ? selectedValue : "N";
                if(item.CauHoi.DapAnChinhXac.ToLower().Contains(item.DapAnChon.ToLower()))
                {
                    correct++;
                }
              
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
            List<KetQua> model = db.KetQuas.Where(c => c.MaDe == made && c.Username == username && c.status==true).ToList();
            return View(model);
        }




    }
}