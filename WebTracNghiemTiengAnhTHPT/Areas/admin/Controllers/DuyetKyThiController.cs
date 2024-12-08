using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class DuyetKyThiController : Controller
    {
        // GET: admin/DuyetKyThi

        public ActionResult Index()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {

                ViewBag.KyThiDaDuyet = db.KyThis.Where(c => c.CongKhai == 2).ToList();
                ViewBag.KyThiChoDuyet = db.KyThis.Where(c => c.CongKhai == 1).ToList();

                //    model=model.Where(item => item.UsernameTacGia == Session["UserName"].ToString()).ToList();  
                return View();
            }


        }
        public JsonResult ChangeStatus(int made, int isActive)
        {
            try
            {
                var gg = made;
                TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
                List<KyThi> model = db.KyThis.ToList();
                // Assuming you have a DbContext for accessing the database
                var user = model.SingleOrDefault(u => u.MaDe == gg);
                if (user != null)
                {
                    user.CongKhai = isActive;
                    db.SaveChanges();
                }
                TempData["SuccessMessage"] = "Bạn đã thay đổi trạng thái thành công.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult ChiTietDeThi(int made)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Get the current exam's questions
                var model = context.CauHois
                    .Include(c => c.NhomCauHoi).Include(c => c.DangBais)
                    .Where(item => item.KyThis.Any(c => c.MaDe == made && c.isDeleted != true))
                    .ToList();

                // Get questions that are not part of the current exam
                var otherQuestions = context.CauHois
                    .Where(item => !item.KyThis.Any(c => c.MaDe == made && c.isDeleted != true))
                    .ToList();

                var kythi = context.KyThis.FirstOrDefault(k => k.MaDe == made);
                ViewBag.KyThi = kythi;
                ViewBag.made = made;
                ViewBag.OtherQuestions = otherQuestions; // Pass other questions to the view

                var firstCauHoi = kythi.CauHois.FirstOrDefault();
                if (firstCauHoi != null)
                {
                    ViewBag.Currenclass = firstCauHoi.DangBais.Where(c => c.TenLoai.Contains("Lớp")).FirstOrDefault()?.TenLoai ?? "No Class";
                    ViewBag.CurrenBoSach = firstCauHoi.DangBais.Where(c => c.TenLoai == "Friends Global" || c.TenLoai == "Global Success" || c.TenLoai == "Smart World").FirstOrDefault()?.TenLoai ?? "No Book Type";
                }
                else
                {
                    ViewBag.Currenclass = "chưa chọn lớp";
                    ViewBag.CurrenBoSach = "chưa chọn sách";
                }

                ViewBag.dangBais = context.DangBais.ToList();
                return View(model);
            }
        }
        public ActionResult ReportError(int MaDe, int MaCauHoi, string ErrorMessage)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            // Validate input parameters
            if (MaDe <= 0 || MaCauHoi <= 0 || string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            // Retrieve the username from the authenticated user
            string username = Session["UserName"]?.ToString();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Json(new { success = false, message = "Người dùng chưa xác thực." });
            }

            // Check if the MaDe exists in KyThi table
            var kyThiExists = db.KyThis.Any(k => k.MaDe == MaDe);
            if (!kyThiExists)
            {
                return Json(new { success = false, message = kyThiExists });
            }



            // Create a new BaoLoi entry
            BaoLoi errorReport = new BaoLoi();
            errorReport.NoiDung = ErrorMessage;
            errorReport.Username = username;
            errorReport.MaCauHoi = MaCauHoi;
            errorReport.MaDe = MaDe;

            DateTime newdate = DateTime.UtcNow;
            newdate = DateTimeHelper.ConvertToLocalTime(newdate);

            errorReport.ThoiGian = newdate;
            db.BaoLois.Add(errorReport);
            db.SaveChanges();

            // Return a success response
            return Json(new { success = true, message = "Error report submitted successfully!" });


        }
    }
}