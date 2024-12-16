using System;
using System.Linq;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;
using System.Data.Entity;
using System.Globalization;

namespace WebTracNghiemTiengAnhTHPT.Areas.hocsinh.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();

        // GET: hocsinh/ThongTinCaNhan
        public ActionResult Index()
        {
            var userName = Session["UserName"] as string;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }

            TaiKhoan userInfo = GetUserInfoByUserName(userName);

            if (userInfo == null)
            {
                // Log to check
                System.Diagnostics.Debug.WriteLine("Không tìm thấy người dùng.");
                return HttpNotFound();
            }

            // Log to check model
            System.Diagnostics.Debug.WriteLine($"Thông tin người dùng: {userInfo.HoTen}");

            return View(userInfo);
        }

        public ActionResult ThongTinCaNhan()
        {
            var userName = Session["UserName"] as string;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }

            TaiKhoan userInfo = GetUserInfoByUserName(userName);

            if (userInfo == null)
            {
                // Log to check
                System.Diagnostics.Debug.WriteLine("Không tìm thấy người dùng.");
                return HttpNotFound();
            }

            // Log to check model
            System.Diagnostics.Debug.WriteLine($"Thông tin người dùng: {userInfo.HoTen}");

            return View(userInfo);
        }

        private TaiKhoan GetUserInfoByUserName(string userName)
        {
            return db.TaiKhoans.FirstOrDefault(u => u.Username == userName); // Adjust table and column names according to your database
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {
                    var username = Session["UserName"].ToString() ?? "";
                    var user = context.TaiKhoans.FirstOrDefault(u => u.Username == username);
                    if (user != null)
                    {
                        user.HoTen = model.HoTen;
                        user.Gmail = model.Gmail;
                        user.SoDienThoai = model.SoDienThoai;
                        user.NgaySinh = model.NgaySinh;
                        user.DiaChi = model.DiaChi;

                        context.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }
        public JsonResult GetKetQuaData()
        {
            var userName = Session["UserName"] as string;
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            var ketQuaData = db.KetQuas.Where(k=>k.Username ==userName)
                .OrderBy(k => k.thoigian_batdau)
                .ToList() // Execute the query and bring data into memory
                .Select(k => new
                {
                    ThoiGianBatDau = k.thoigian_batdau.HasValue ? k.thoigian_batdau.Value.ToString("yyyy-MM-dd") : string.Empty,
                    Diem = k.Diem ?? 0,
                    TenKyThi = k.KyThi.TenKyThi
                })
                .ToList();

            return Json(ketQuaData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetResults(string date)
        {
            string format = "dd/MM/yyyy";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime selectedDate;
            if (DateTime.TryParseExact(date, format, provider, DateTimeStyles.None, out selectedDate))
            {
                var results = db.KetQuas
                    .Where(kq => kq.thoigian_batdau.HasValue  &&
                                 DbFunctions.TruncateTime(kq.thoigian_batdau) == selectedDate.Date)
                    .Select(kq => new
                    {
                        examName = kq.KyThi.TenKyThi,
                        score = kq.Diem,
                        status = kq.status,
                        maKQ = kq.Maketqua,
                        startTime = kq.thoigian_batdau,
                        endTime = kq.thoigian_ketthuc
                    })
                    .ToList();

                return Json(results, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Invalid date format." }, JsonRequestBehavior.AllowGet);
        }

    }
}
