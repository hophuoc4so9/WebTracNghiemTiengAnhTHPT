using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyCauHoiController : Controller
    {
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
        // GET: giaovien/QuanLyCauHoi
        public ActionResult Index()
        {
            List<CauHoi> model = db.CauHois.ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddCauHoi(CauHoi model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo đối tượng câu hỏi mới
                    var cauHoi = new CauHoi
                    {
                        MaCauHoi = model.MaCauHoi,
                        NoiDung = model.NoiDung,
                        DapAnA = model.DapAnA,
                        DapAnB = model.DapAnB,
                        DapAnC = model.DapAnC,
                        DapAnD = model.DapAnD,
                        DapAnChinhXac = model.DapAnChinhXac,
                        MaNhom = model.MaNhom,
                        MucDo = model.MucDo,
                        // Các thuộc tính khác nếu có
                    };

                    // Thêm vào cơ sở dữ liệu
                    db.CauHois.Add(cauHoi);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu." });
                }
            }

            // Nếu ModelState không hợp lệ, thu thập các lỗi
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                   .ToDictionary(
                                       kvp => kvp.Key,
                                       kvp => kvp.Value.Errors.First().ErrorMessage
                                   );

            return Json(new { success = false, errors = errors });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult Delete(string maCauHoi, string made)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var cauHoi = db.CauHois.FirstOrDefault(k => k.MaCauHoi == maCauHoi);

                if (cauHoi == null)
                {
                    return HttpNotFound(); // Nếu không tìm thấy câu hỏi
                }

                // Nếu tìm thấy, xóa câu hỏi
                db.CauHois.Remove(cauHoi);
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Chuyển hướng về trang danh sách câu hỏi
                return RedirectToAction("Index", "QuanLyCauHoi"); // Chuyển hướng về action Index của controller QuanLyCauHoi
            }
        }
        // GET: Hiển thị form chỉnh sửa
        public ActionResult Edit(string maCauHoi)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var cauHoi = db.CauHois.FirstOrDefault(k => k.MaCauHoi == maCauHoi);
                if (cauHoi == null)
                {
                    return HttpNotFound(); // Nếu không tìm thấy câu hỏi
                }
                ViewBag.NhomList = new SelectList(db.NhomCauHois.ToList(), "MaNhom", "MaNhom");

                return View(cauHoi); // Trả về view chỉnh sửa với câu hỏi
            }
        }

        // POST: Lưu thay đổi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CauHoi cauHoi)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cauHoi).State = EntityState.Modified; // Đánh dấu câu hỏi là đã chỉnh sửa
                    db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    // Chuyển hướng về trang danh sách câu hỏi
                    return RedirectToAction("Index", "QuanLyCauHoi");
                }
                return View(cauHoi); // Nếu có lỗi, quay lại form chỉnh sửa
            }
        }
        public ActionResult Details(string maCauHoi)
        {
            var cauHoi = db.CauHois.Find(maCauHoi);
            if (cauHoi == null)
            {
                return HttpNotFound();
            }

            // Khởi tạo danh sách nhóm
            ViewBag.NhomList = new SelectList(db.NhomCauHois.ToList(), "MaNhom", "NoiDung");

            // Tạo danh sách mức độ
            ViewBag.MucDoList = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Dễ" },
        new SelectListItem { Value = "2", Text = "Trung bình" },
        new SelectListItem { Value = "3", Text = "Khó" }
    }, "Value", "Text");

            return View(cauHoi);
        }

    }
}