﻿using Microsoft.Ajax.Utilities;
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
using System.Text.RegularExpressions;
using System.Threading;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Aspose.Words;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;


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
        public ActionResult ChiTietDeThi(int made)
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
        public ActionResult AddQuestionToExam(int made, int maCauHoi)
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
        public ActionResult DeleteCauHoi(int maCauHoi, int made)
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
                                var batDauDate = form[$"KyThi[{index}].ThoiGianBatDau"];
                                kyThi.ThoiGianBatDau = string.IsNullOrEmpty(batDauDate) 
                                    ? (DateTime?)null
                                    : DateTime.Parse(batDauDate);

                                // Update ThoiGianKetThuc
                                var ketThucDate = form[$"KyThi[{index}].ThoiGianKetThuc"];
                                kyThi.ThoiGianKetThuc = string.IsNullOrEmpty(ketThucDate) 
                                    ? (DateTime?)null
                                    : DateTime.Parse(ketThucDate);

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

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
            {
            string contents = string.Empty;

            try
            {
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    ms.Position = 0;

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (fileExtension == ".docx")
                    {
                        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false))
                        {
                            DocumentFormat.OpenXml.Wordprocessing.Body body = wordDoc.MainDocumentPart.Document.Body;
                            contents = body.InnerText;
                        }
                    }
                    else if (fileExtension == ".doc")
                    {
                        Aspose.Words.Document doc = new Aspose.Words.Document(ms);
                        contents = doc.ToString(SaveFormat.Text);
                    }
                    else if (fileExtension == ".pdf")
                    {
                        using (PdfReader reader = new PdfReader(ms))
                        using (PdfDocument pdfDoc = new PdfDocument(reader))
                        {
                            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                            {
                                contents += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                            }
                        }
                    }
                    else
                    {
                        throw new FileFormatException("Unsupported file format.");
                    }
                }

                List<NhomCauHoi> questions = ProcessExamText(contents);
                Session["questions"] = questions;
            }
            catch (Exception ex)
            {
                // Log the exception for further analysis
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                return View("Error");
            }

            return View("AddFile");
        }

        private List<NhomCauHoi> ProcessExamText(string text)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<NhomCauHoi> nhomCauHois = db.NhomCauHois.ToList();
            var questionPattern = @"Question\s\d+.*?(?=Question\s\d+|$)";
            var questionMatches = Regex.Matches(text, questionPattern, RegexOptions.Singleline);
            bool newGroup = true;
            int groupId = 1;
            int index = 1;
            string newGroupContent = "";

            foreach (Match match in questionMatches)
            {
                var questionText = match.Value;
                var parts = Regex.Split(questionText, @"(?=A\.\s|B\.\s|C\.\s|D\.\s)");

                if (newGroup && newGroupContent!="")
                {
                    NhomCauHoi nhomCauHoi = new NhomCauHoi
                    {
                        NoiDung = groupId == 1 ? text.Substring(0, match.Index).Trim() : newGroupContent
                    };
                    nhomCauHois.Add(nhomCauHoi);
                    index = nhomCauHoi.MaNhom;
                    newGroup = false;
                }

                if (parts.Length < 5)
                {
                    // Handle error: not enough parts
                    continue;
                }

                if (parts[4].Contains("\n") && !Regex.IsMatch(parts[4], @"Question\s\d+"))
                {
                    newGroup = true;
                    groupId++;
                    newGroupContent = parts[4].Substring(parts[4].IndexOf("\n")).Trim();
                    parts[4] = parts[4].Substring(0, parts[4].IndexOf("\n")).Trim();
                }

                var cauHoi = new CauHoi
                {
                    NoiDung = parts[0].Trim(),
                    DapAnA = parts[1].Substring(2).Trim(),
                    DapAnB = parts[2].Substring(2).Trim(),
                    DapAnC = parts[3].Substring(2).Trim(),
                    DapAnD = parts[4].Substring(2).Trim(),
                    MaNhom = index
                    
                };
                
                nhomCauHois.Last().CauHois.Add(cauHoi);
            }
            //nếu newGroupContent có dạng Đáp án 1.A 2.B 3.D thì cặp nhật câu hỏi thứ i cauHoi.DapAnChinhXac=A
            return nhomCauHois;
        }
   
}






}