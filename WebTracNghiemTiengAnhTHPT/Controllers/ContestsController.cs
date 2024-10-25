using DocumentFormat.OpenXml.Drawing.Charts;
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
        private readonly TracNghiemTiengAnhTHPTEntities1 _db;

        // Parameterless constructor
        public ContestsController() : this(new TracNghiemTiengAnhTHPTEntities1())
        {
        }

        public ContestsController(TracNghiemTiengAnhTHPTEntities1 db)
        {
            _db = db;
        }

        // GET: Contests
        public ActionResult Index(int? PhongThiId)
        {
            
            if(PhongThiId!=null)
            {
                PhongThi phong = _db.PhongThis.FirstOrDefault(n => n.MaPhong == PhongThiId);
                List<ViewChitietKyThi_2> list =new List<ViewChitietKyThi_2>();
                foreach(KyThi item in phong.KyThis)
                {
                    ViewChitietKyThi_2 tam = new ViewChitietKyThi_2();
                    tam.MaDe = item.MaDe;
                    tam.TenKyThi = item.TenKyThi;
                    tam.ThoiGian = item.ThoiGian;  
                    tam.ThoiGianBatDau=item.ThoiGianBatDau;
                    tam.ThoiGianKetThuc = item.ThoiGianKetThuc;
                    tam.SoCauHoi = item.SoCauHoi;
                    tam.Soluot = item.KetQuas.Count(); 
                     if(item.KetQuas.Count()>0)     tam.DiemTrungBinh = (int)item.KetQuas.Average(k => k.Diem);
                    else tam.DiemTrungBinh = 0;
                    list.Add(tam);

                }    
                return View(list);
            }    
          
            
            var model = _db.ViewChitietKyThi_2.AsNoTracking().ToList();
            return View(model);
        }

        public ActionResult ChiTietKyThi(int made)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Login", new { area = "admin" });
            }

            string username = Session["UserName"].ToString();
            var ketQua = _db.KetQuas
            .AsNoTracking()
            .FirstOrDefault(k => k.Username == username && k.status == false && k.thoigian_ketthuc > DateTime.Now);

            if (ketQua != null)
            {
                if (ketQua.MaDe != made)
                {
                    ViewBag.Message = "Bạn đang ở trong một kỳ thi khác nên không thể tham gia.";
                }
                ViewBag.endTime = ketQua.thoigian_ketthuc;
                return RedirectToAction("BaiLam", new { id = ketQua.Maketqua });
            }

            var kt = _db.KyThis.SingleOrDefault(k => k.MaDe == made);
            if (kt == null)
            {
                return HttpNotFound();
            }

            if (!kt.SoCauHoi.HasValue || kt.SoCauHoi <= 0)
            {
                kt.SoCauHoi = kt.CauHois.Count();
            }

            ketQua = new KetQua
            {
                Username = username,
                MaDe = made,
                status = false,
                thoigian_batdau = DateTime.Now,
                thoigian_ketthuc = DateTime.Now.AddMinutes(kt.ThoiGian)
            };

            _db.KetQuas.Add(ketQua);
            _db.SaveChanges();

            var shuffledQuestions = kt.CauHois.OrderBy(q => Guid.NewGuid()).Take(kt.SoCauHoi ?? 0).ToList();

            foreach (var question in shuffledQuestions)
            {
                var chiTietKetQua = new ChiTietKetQua
                {
                    MaCauHoi = question.MaCauHoi,
                    DapAnChon = "N",
                    Username = username,
                    MaDe = made,
                    Maketqua = ketQua.Maketqua
                };
                ketQua.ChiTietKetQuas.Add(chiTietKetQua);
            }

            _db.SaveChanges();

            return RedirectToAction("BaiLam", new { id = ketQua.Maketqua });
        }

        public ActionResult BaiLam(int id)
        {
            var kq = _db.KetQuas.AsNoTracking().SingleOrDefault(k => k.Maketqua == id);
            if (kq == null)
            {
                return HttpNotFound();
            }

            ViewBag.MaDe = kq.Maketqua;
            ViewBag.endTime = kq.thoigian_ketthuc;
            return View(kq);
        }

        [HttpPost]
        public ActionResult Result(FormCollection form, int made)
        {
            var ketqua = _db.KetQuas.SingleOrDefault(n => n.Maketqua == made);
            if (ketqua == null)
            {
                return HttpNotFound();
            }

            if (int.Parse(form["flag"]) == 0 || ketqua.thoigian_ketthuc <= DateTime.Now)
            {
                ketqua.status = true;
            }

            int total = ketqua.ChiTietKetQuas.Count;
            double correct = 0;

            foreach (var item in ketqua.ChiTietKetQuas)
            {
                string questionKey = "answer_" + item.MaCauHoi;
                string selectedValue = form[questionKey];
                item.DapAnChon = !string.IsNullOrEmpty(selectedValue) ? selectedValue : "N";
                if (item.CauHoi.DapAnChinhXac.ToLower().Contains(item.DapAnChon.ToLower()))
                {
                    correct++;
                }
            }

            ketqua.Diem = correct * 10 / total;
            if (int.Parse(form["flag"]) == 0 || ketqua.thoigian_ketthuc <= DateTime.Now)
            {
                ketqua.status = true;
            }

            _db.SaveChanges();

            if (ketqua.status == false && int.Parse(form["flag"]) == 0 && !string.IsNullOrEmpty(form["url"]))
            {
                return Redirect(form["url"]);
            }

            return RedirectToAction("Result", new { maketqua = ketqua.Maketqua });
        }

        [HttpGet]
        public ActionResult Result(int maketqua)
        {
            var kq = _db.KetQuas.AsNoTracking().SingleOrDefault(k => k.Maketqua == maketqua);
            if (kq == null)
            {
                return HttpNotFound();
            }

            return View(kq);
        }

        [HttpGet]
        public ActionResult ChiTiet(int made)
        {
            var ky = _db.KyThis.AsNoTracking().SingleOrDefault(k => k.MaDe == made);
            if (ky == null)
            {
                return HttpNotFound();
            }

            ViewBag.MaDe = made;
            return View(ky);
        }

        public ActionResult PartialDanhGiaKyThi(int made)
        {
            var model = _db.DanhGias.AsNoTracking().Where(c => c.MaDe == made).ToList();
            return View(model);
        }

        public ActionResult PartialLichSuLamBai(int made)
        {
            string username = Session["UserName"]?.ToString() ?? string.Empty;
            var model = _db.KetQuas.AsNoTracking().Where(c => c.MaDe == made && c.Username == username && c.status == true).ToList();
            return View(model);
        }
        public ActionResult PartialLichSuLamBaiOnTap()
        {
            string username = Session["UserName"]?.ToString() ?? string.Empty;
            var model = _db.KetQuas.AsNoTracking().Where(c =>c.Username == username && c.status == true && c.KyThi.UsernameTacGia==username).ToList();
            return View("PartialLichSuLamBai", model);

        }
        public ActionResult PartialLichSuLamBaiAlL()
        {
            string username = Session["UserName"]?.ToString() ?? string.Empty;
            var model = _db.KetQuas.AsNoTracking().Where(c => c.Username == username && c.status == true).ToList();
            return View("PartialLichSuLamBai", model);

        }
    }
}
