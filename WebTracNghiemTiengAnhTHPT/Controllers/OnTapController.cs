﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class OnTapController : Controller
    {
        // GET: OnTap
        public ActionResult Index()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var totalQuestions = db.CauHois.Where(n=>n.DaDuyet==true).Count();
                var easyQuestions = db.CauHois.Where(n => n.DaDuyet == true).Count(q => q.MucDo == 1);
                var hardQuestions = db.CauHois.Where(n => n.DaDuyet == true).Count(q => q.MucDo == 2);

                ViewBag.TotalQuestions = totalQuestions;
                ViewBag.EasyQuestions = easyQuestions;
                ViewBag.HardQuestions = hardQuestions;
                Session["AllDangBais"] = db.DangBais.ToList();
            }

            return View();
        }

        [HttpPost]
        public ActionResult AutoGenerateExam(int SoCauHoiDe, int SoCauHoiKho, int ThoiGian, string Seleted)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Parse selected DangBai IDs from the hidden input
                var selectedDangBaiIds = !string.IsNullOrEmpty(Seleted) ? Seleted.Split(',').Select(int.Parse).ToList() : null;

                List<DangBai> allDangBais = new List<DangBai>();
                List<CauHoi> easyQuestions = new List<CauHoi>();
                List<CauHoi> hardQuestions = new List<CauHoi>();

                if (selectedDangBaiIds != null && selectedDangBaiIds.Any())
                {
                    allDangBais = db.DangBais.Where(d => selectedDangBaiIds.Contains(d.MaLoai)).ToList();

                    easyQuestions = db.CauHois
                        .Where(q => q.MucDo == 1 && !q.isDeleted && q.DangBais.Any(d => selectedDangBaiIds.Contains(d.MaLoai)))
                        .ToList();
                    hardQuestions = db.CauHois
                        .Where(q => q.MucDo == 2 && !q.isDeleted && q.DangBais.Any(d => selectedDangBaiIds.Contains(d.MaLoai)))
                        .ToList();
                }
                else
                {
                    easyQuestions = db.CauHois
                        .Where(q => q.MucDo == 1 && !q.isDeleted)
                        .ToList();
                    hardQuestions = db.CauHois
                        .Where(q => q.MucDo == 2 && !q.isDeleted)
                        .ToList();
                }

                if (SoCauHoiDe > easyQuestions.Count || SoCauHoiKho > hardQuestions.Count)
                {
                    ModelState.AddModelError("", "Không đủ câu hỏi để tạo đề thi.");
                    TempData["Error"] = "Không đủ câu hỏi để tạo đề thi.";
                    ViewBag.TotalQuestions = db.CauHois.Count();
                    ViewBag.EasyQuestions = easyQuestions.Count;
                    ViewBag.HardQuestions = hardQuestions.Count;
                    return View("Index");
                }

                Random random = new Random();
                var selectedEasyQuestions = easyQuestions
                    .OrderBy(x => random.Next())
                    .Take(SoCauHoiDe)
                    .ToList();
                var selectedHardQuestions = hardQuestions
                    .OrderBy(x => random.Next())
                    .Take(SoCauHoiKho)
                    .ToList();

                var newExam = new KyThi
                {
                    TenKyThi = "Đề Thi Ngẫu Nhiên " + DateTime.UtcNow.ToString("yyyyMMddHHmm") + " - " + Guid.NewGuid().ToString().Substring(0, 8),
                    ThoiGian = ThoiGian,
                    ThoiGianBatDau = DateTime.UtcNow,
                    isDeleted = false,
                    CongKhai = 0,
                    UsernameTacGia = Session["UserName"]?.ToString()
                };
                newExam.SoCauHoi = SoCauHoiDe + SoCauHoiKho;
                db.KyThis.Add(newExam);
                db.SaveChanges();

                foreach (var question in selectedEasyQuestions)
                {
                    newExam.CauHois.Add(question);
                }
                foreach (var question in selectedHardQuestions)
                {
                    newExam.CauHois.Add(question);
                }

                db.SaveChanges();

                TempData["Success"] = "Đề thi đã được tạo thành công!";
                return RedirectToAction("ChiTietKyThi", "Contests", new { made = newExam.MaDe });
            }
        }
    }
}