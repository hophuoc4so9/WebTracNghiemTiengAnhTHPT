using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;
using WebTracNghiemTiengAnhTHPT.Services;

namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class ContestsController : Controller
    {
        private readonly TracNghiemTiengAnhTHPTEntities1 _db;

        // Parameterless constructor
        public ContestsController() : this(new TracNghiemTiengAnhTHPTEntities1())
        {
            DotNetEnv.Env.Load();

        }

        public ContestsController(TracNghiemTiengAnhTHPTEntities1 db)
        {
            DotNetEnv.Env.Load();

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
            Debug.WriteLine($"Made: {made}");

            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Login", new { area = "admin" });
            }
            var danhGia = _db.DanhGias.Where(d => d.MaDe == made).ToList();
            ViewBag.DanhGia = danhGia;
            string username = Session["UserName"].ToString();
            Session["made"] = made; 
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
        [HttpPost]
        public ActionResult DanhGia(int made, int rate, string comment)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Login", new { area = "admin" });
            }

            string username = Session["UserName"].ToString();

            // Check if the user has already rated this exam
            var existingRating = _db.DanhGias
                .FirstOrDefault(d => d.MaDe == made && d.Username == username);

            if (existingRating != null)
            {
                // If already rated, update the existing rating
                existingRating.Rate = rate;
                existingRating.NoiDung = comment;
               
            }
            else
            {
                // If not rated before, create a new rating
                var newRating = new DanhGia
                {
                    MaDe = made,
                    Username = username,
                    Rate = rate,
                    NoiDung = comment,

                };
                _db.DanhGias.Add(newRating);
            }

            _db.SaveChanges();

            return RedirectToAction("ChiTietKyThi", new { made = made });
        }

        public ActionResult BaiLam(int id)
        {
            var kq = _db.KetQuas.AsNoTracking().SingleOrDefault(k => k.Maketqua == id);
            if (kq == null)
            {
                return HttpNotFound();
            }
            if(kq.status==true)
            {
                return RedirectToAction("Result", new { maketqua = kq.Maketqua });
            }    
            ViewBag.MaDe = kq.Maketqua;
            ViewBag.MaDeReal = Session["made"]; 
            ViewBag.endTime = kq.thoigian_ketthuc;
            return View(kq);
        }
        [HttpPost]
 
        public ActionResult ReportError(int MaDe, int MaCauHoi, string ErrorMessage)
        {
            // Validate input parameters
            if (MaDe <= 0 || MaCauHoi <= 0 || string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            // Retrieve the username from the authenticated user
            string username = Session["UserName"]?.ToString();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            // Check if the MaDe exists in KyThi table
            var kyThiExists = _db.KyThis.Any(k => k.MaDe == MaDe);
            if (!kyThiExists)
            {
                return Json(new { success = false, message = kyThiExists});
            }

      

            // Create a new BaoLoi entry
            BaoLoi errorReport = new BaoLoi(); 
            errorReport.NoiDung = ErrorMessage;
            errorReport.Username = username;
            errorReport.MaCauHoi = MaCauHoi;
            errorReport.MaDe = MaDe; 

                _db.BaoLois.Add(errorReport);
                _db.SaveChanges();

                // Return a success response
                return Json(new { success = true, message = "Error report submitted successfully!" });
            
  
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
            var danhGia = _db.DanhGias.Where(d => d.MaDe == made).ToList();
            ViewBag.DanhGia = danhGia;
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
            return View("PartialBieuDoLichSuLamBai", model);

        }
        public async Task< ActionResult> GiveAdvice(int maketqua)
        {
            var kq = _db.KetQuas.AsNoTracking().SingleOrDefault(k => k.Maketqua == maketqua);
            if (kq == null)
            {
                return HttpNotFound();
            }
            string apiKey = Environment.GetEnvironmentVariable("API_KEY_GROQ");

            var _groqService = new GroqService("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");

            StringBuilder promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("Give advice:");

            bool hasIncorrectAnswers = false;

            foreach (var item in kq.ChiTietKetQuas)
            {
                var cauhoi = _db.CauHois.AsNoTracking().SingleOrDefault(c => c.MaCauHoi == item.MaCauHoi);
                item.CauHoi = cauhoi;

                string cauhois = item.CauHoi.NhomCauHoi.NoiDung + item.CauHoi.NoiDung;
                string dapana = item.CauHoi.DapAnA;
                string dapanb = item.CauHoi.DapAnB;
                string dapanc = item.CauHoi.DapAnC;
                string dapand = item.CauHoi.DapAnD;
                string dapanchon = item.DapAnChon;
                string dapanchinhxac = item.CauHoi.DapAnChinhXac;

                // Only include incorrect answers
                if (!dapanchinhxac.Equals(dapanchon, StringComparison.OrdinalIgnoreCase))
                {
                    hasIncorrectAnswers = true;
                    promptBuilder.AppendLine($"Question: {cauhois}");
                    promptBuilder.AppendLine($"A: {dapana}");
                    promptBuilder.AppendLine($"B: {dapanb}");
                    promptBuilder.AppendLine($"C: {dapanc}");
                    promptBuilder.AppendLine($"D: {dapand}");
                    promptBuilder.AppendLine($"Selected Answer: {dapanchon}");
                    promptBuilder.AppendLine($"Correct Answer: {dapanchinhxac}");
                    promptBuilder.AppendLine();
                }
            }

            if (!hasIncorrectAnswers)
            {
                return Content("Bạn làm tốt lắm!");
            }

            // Get advice from the API
            string prompt = promptBuilder.ToString();
            JsonObject request = new JsonObject
            {
                ["model"] = "mixtral-8x7b-32768",
                ["messages"] = new JsonArray
        {
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = prompt,
            }
        }
            };

            try
            {
                var response = await _groqService.CreateChatCompletionAsync(request);
                string advice = response?["choices"]?[0]?["message"]?["content"]?.ToString();
                return PartialView("GiveAdvice", advice);
            }
            catch (Exception ex)
            {
                // Log or handle error as needed
                return PartialView("GiveAdvice", $"An error occurred: {ex.Message}");
               
            }

         
        }

        public ActionResult PartialDangBai(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                KyThi kyThi = context.KyThis.Find(id);
                if (kyThi == null)
                {
                    return HttpNotFound();
                }

                var allDangBai = new List<DangBai>();
                var cauHois = kyThi.CauHois.ToList();

                foreach (var cauHoi in cauHois)
                {
                    foreach (var dangBai in cauHoi.DangBais)
                    {
                        allDangBai.Add(dangBai);
                    }
                }

                // Group by DangBai and calculate the percentage
                var groupedDangBai = allDangBai
                    .GroupBy(db => db.MaLoai)
                    .Select(g => new
                    {
                        DangBai = g.First(),
                        Percentage = (double)g.Count() / allDangBai.Count * 100
                    })
                    .Where(g => g.Percentage > 50)
                    .Select(g => g.DangBai)
                    .ToList();

                return PartialView(groupedDangBai);
            }
        }



       


    }
}
