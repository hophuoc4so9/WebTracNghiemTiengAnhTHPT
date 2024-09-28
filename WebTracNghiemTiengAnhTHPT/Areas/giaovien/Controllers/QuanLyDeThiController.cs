using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebTracNghiemTiengAnhTHPT.Models;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyDeThiController : Controller
    {
        // GET: giaovien/QuanLyDeThi
        public ActionResult Index()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<KyThi> model = db.KyThis.ToList();
                //model=model.Where(item => item.UsernameTacGia == Session["UserName"].ToString()).ToList();  
                return View(model);
            }

           
        }
        public ActionResult ThemMoiDeThi()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                KyThi model = new KyThi();
                return View(model);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoiDeThi(KyThi model)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                if (ModelState.IsValid)
                {
                    // Check if MaDe is unique
                    var existingKyThi = db.KyThis.FirstOrDefault(k => k.MaDe == model.MaDe);
                    if (Session["UserName"] != null) model.UsernameTacGia = Session["UserName"].ToString();
                    if (existingKyThi != null)
                    {
                        // Add error to ModelState
                        ModelState.AddModelError("MaDe", "Mã Đề đã tồn tại, vui lòng nhập mã khác.");
                        return View(model);
                    }

                    // If unique, proceed with adding the new KyThi
                    db.KyThis.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

              
                return View(model);
            }    
               
        }
        public ActionResult ChiTietDeThi(string made)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Get the current exam's questions
                var model = context.CauHois
                    .Include(c => c.NhomCauHoi)
                    .Where(item => item.KyThis.Any(c => c.MaDe == made && c.isDeleted!=true))
                    .ToList();

                // Get questions that are not part of the current exam
                var otherQuestions = context.CauHois
                    .Where(item => !item.KyThis.Any(c => c.MaDe == made && c.isDeleted != true))
                    .ToList();

                ViewBag.made = made;
                ViewBag.OtherQuestions = otherQuestions; // Pass other questions to the view
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult AddQuestionToExam(string made, string maCauHoi)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Find the exam and question
                var exam = context.KyThis.FirstOrDefault(k => k.MaDe == made);
                var question = context.CauHois.FirstOrDefault(q => q.MaCauHoi == maCauHoi);

                if (exam != null && question != null)
                {
                    // Add the question to the exam (this depends on your model relationships)
                    exam.CauHois.Add(question);
                    context.SaveChanges();
                }

                // Redirect back to the ChiTietDeThi action to reload the view
                return RedirectToAction("ChiTietDeThi", new { made });
            }
        }
        [HttpPost]
        public ActionResult DeleteCauHoi(string maCauHoi, string made)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var kyThi = db.KyThis.Include(k => k.CauHois)
                                 .FirstOrDefault(k => k.MaDe == made);
            if (kyThi == null)
            {
                return HttpNotFound();
            }

            // Find the CauHoi in the KyThi
            var cauHoiToRemove = kyThi.CauHois.FirstOrDefault(ch => ch.MaCauHoi == maCauHoi);
            if (cauHoiToRemove != null)
            {
                kyThi.CauHois.Remove(cauHoiToRemove);
                db.SaveChanges();
            }
            return RedirectToAction("ChiTietDeThi", new { made });
            }
        }
        [HttpPost]
        public ActionResult Update(FormCollection form)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Loop through the KyThi entries
                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("KyThi["))
                    {
                        // Extract the index
                        var indexStart = key.IndexOf("[") + 1;
                        var indexEnd = key.IndexOf("]", indexStart);
                        var indexString = key.Substring(indexStart, indexEnd - indexStart);
                        int index;

                        if (int.TryParse(indexString, out index))
                        {
                            // Get the MaDe value to find the specific KyThi item
                            var maDe = form[$"KyThi[{index}].MaDe"];
                            var kyThi = db.KyThis.Find(maDe);

                            if (kyThi != null)
                            {
                                // Update the fields
                                kyThi.TenKyThi = form[$"KyThi[{index}].TenKyThi"];
                                kyThi.ThoiGian = int.TryParse(form[$"KyThi[{index}].ThoiGian"], out var thoiGian) ? thoiGian : 0;

                                // Update ThoiGianBatDau
                                var batDauDate = form[$"KyThi[{index}].ThoiGianBatDauDate"];
                                var batDauTime = form[$"KyThi[{index}].ThoiGianBatDauTime"];
                                kyThi.ThoiGianBatDau = string.IsNullOrEmpty(batDauDate) || string.IsNullOrEmpty(batDauTime)
                                    ? (DateTime?)null
                                    : DateTime.Parse($"{batDauDate} {batDauTime}");

                                // Update ThoiGianKetThuc
                                var ketThucDate = form[$"KyThi[{index}].ThoiGianKetThucDate"];
                                var ketThucTime = form[$"KyThi[{index}].ThoiGianKetThucTime"];
                                kyThi.ThoiGianKetThuc = string.IsNullOrEmpty(ketThucDate) || string.IsNullOrEmpty(ketThucTime)
                                    ? (DateTime?)null
                                    : DateTime.Parse($"{ketThucDate} {ketThucTime}");

                                // Update CongKhai   KyThi[@item.MaDe].CongKhai
                                kyThi.CongKhai = form[$"KyThi[{index}].CongKhai"] != null && form[$"KyThi[{index}].CongKhai"] == "on";

                               if(Session["UserName"]!=null)   kyThi.UsernameTacGia = Session["UserName"].ToString();

                                // Handle isDeleted
                                kyThi.isDeleted = form[$"KyThi[{index}].isDeleted"] == "true"; // Set as deleted if true
                            }
                        }
                    }
                }

                // Save changes to the database
                db.SaveChanges();
            }

            // Redirect or return a view as appropriate
            return RedirectToAction("Index");
        }


      

    }
}