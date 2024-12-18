﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        // GET: Hiển thị form chỉnh sửa
        public ActionResult Edit(int maCauHoi)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var cauHoi = db.CauHois.FirstOrDefault(k => k.MaCauHoi == maCauHoi);
                if (cauHoi == null)
                {
                    return HttpNotFound();
                }
                ViewBag.NhomList = new SelectList(db.NhomCauHois.ToList(), "MaNhom", "MaNhom");

                return View(cauHoi);
            }
        }

        [HttpPost]
        public ActionResult Edit(CauHoi cauHoi)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cauHoi).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index", "QuanLyCauHoi");
                }
                return View(cauHoi);
            }
        }
        public ActionResult Details(int maCauHoi)
        {
            var cauHoi = db.CauHois.Find(maCauHoi);
            if (cauHoi == null)
            {
                return HttpNotFound();
            }

            ViewBag.NhomList = new SelectList(db.NhomCauHois.ToList(), "MaNhom", "NoiDung");

            ViewBag.MucDoList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Dễ" },
                new SelectListItem { Value = "2", Text = "Trung bình" },
                new SelectListItem { Value = "3", Text = "Khó" }
            }, "Value", "Text");

            return View(cauHoi);
        }
        public ActionResult TimKiem(string query)
        {
            var cauHois = db.CauHois
                .Where(c => c.NoiDung.Contains(query) || c.MaCauHoi.ToString().Contains(query))
                .ToList();

            string result = "";
            foreach (var cauHoi in cauHois)
            {
                result += $"<tr>" +
                          $"<td>{cauHoi.MaCauHoi}</td>" +
                          $"<td>{cauHoi.NoiDung}</td>" +
                          $"<td>{cauHoi.MaNhom}</td>" +
                          $"<td>{cauHoi.MucDo}</td>" +
                          $"<td>{cauHoi.DapAnChinhXac}</td>" +
                          $"<td>" +
                          $"<a class='btn btn-warning' href='/QuanLyCauHoi/Edit/{cauHoi.MaCauHoi}'>Sửa</a> " +
                          $"<form action='/QuanLyCauHoi/Delete' method='post' style='display:inline;'>" +
                          $"<input type='hidden' name='maCauHoi' value='{cauHoi.MaCauHoi}' />" +
                          $"<button type='submit' class='btn btn-danger' onclick='return confirm(\"Bạn có chắc chắn muốn xóa câu hỏi này không?\");'>Xóa</button>" +
                          $"</form> " +
                          $"<a class='btn btn-success' href='/QuanLyCauHoi/Details/{cauHoi.MaCauHoi}'>Chi tiết</a>" +
                          $"</td>" +
                          $"</tr>";
            }

            return Content(result);
        }
        [HttpPost]
        public ActionResult Delete(int maCauHoi)
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


        // Chức năng tự động sinh đề thi
    }
}