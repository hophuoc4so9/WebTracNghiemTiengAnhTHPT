using Aspose.Words;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;


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
                model=model.Where(item => item.UsernameTacGia == Session["UserName"].ToString()).ToList();  
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
                    ViewBag.Currenclass = "No Class";
                    ViewBag.CurrenBoSach = "No Book Type";
                }

                ViewBag.dangBais = context.DangBais.ToList();
                return View(model);
            }
        }
        public ActionResult PartialDangBai(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                var excludedCategories = new List<string>
        {
            "Lớp 10", "Lớp 11", "Lớp 12", "Listening",
            "Friends Global", "Global Success", "Reading", "Smart World"
        };

                var cauHoi = context.CauHois
                    .Include(c => c.DangBais)
                    .FirstOrDefault(n => n.MaCauHoi == id);

                var allDangBais = context.DangBais
                    .Where(db => !excludedCategories.Contains(db.TenLoai))
                    .ToList();

                ViewBag.AllDangBais = allDangBais;
                return PartialView(cauHoi);
            }
        }

        [HttpPost]
        public ActionResult AddDangBai(int cauHoiId, int dangBaiId)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                var cauHoi = context.CauHois.Include(c => c.DangBais).FirstOrDefault(c => c.MaCauHoi == cauHoiId);
                var dangBai = context.DangBais.FirstOrDefault(d => d.MaLoai == dangBaiId);

                if (cauHoi != null && dangBai != null)
                {
                    cauHoi.DangBais.Add(dangBai);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("PartialDangBai", new { id = cauHoiId });
        }

        [HttpPost]
        public ActionResult RemoveDangBai(int cauHoiId, int dangBaiId)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                var cauHoi = context.CauHois.Include(c => c.DangBais).FirstOrDefault(c => c.MaCauHoi == cauHoiId);
                var dangBai = cauHoi?.DangBais.FirstOrDefault(d => d.MaLoai == dangBaiId);

                if (cauHoi != null && dangBai != null)
                {
                    cauHoi.DangBais.Remove(dangBai);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("PartialDangBai", new { id = cauHoiId });
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
                            int maDe = int.Parse(form[$"KyThi[{index}].MaDe"]);
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

                                if (Session["UserName"] != null) kyThi.UsernameTacGia = Session["UserName"].ToString();

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

            KyThi ktthi = new KyThi();
            ktthi.TenKyThi = Path.ChangeExtension(file.FileName, null);
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
                TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
                //db.KyThis.Add(ktthi);
                //db.SaveChanges();
                ProcessExamText(contents, ktthi);

            }
            catch (Exception ex)
            {
                // Log the exception for further analysis
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                return View("Error");
            }

            return RedirectToAction("ChiTietDeThi", new { made = ktthi.MaDe });
        }

        private void ProcessExamText(string text, KyThi kt)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var questionPattern = @"Question\s\d+.*?(?=Question\s\d+|$)";
                var questionMatches = Regex.Matches(text, questionPattern, RegexOptions.Singleline);
                bool newGroup = true;
                bool fi = true;
                string newGroupContent = "";
                int maxMaCauHoi = db.CauHois.Max(ch => (int?)ch.MaCauHoi) ?? 1;
                int maxgr = db.NhomCauHois.Max(ch => (int?)ch.MaNhom) ?? 1;
                NhomCauHoi nhomcauhoilast = new NhomCauHoi();
                List<CauHoi> cauhois123 = new List<CauHoi>();

                foreach (Match match in questionMatches)
                {
                    var questionText = match.Value;

                    var parts = Regex.Split(questionText, @"(?=A\.\s|B\.\s|C\.\s|D\.\s)");
                    if (parts.Length < 5)
                    {
                        continue;
                    }

                    if (newGroup)
                    {
                        NhomCauHoi nhomCauHoi = new NhomCauHoi
                        {
                            NoiDung = fi ? text.Substring(0, match.Index).Trim() : newGroupContent
                        };
                        maxgr++;
                        nhomCauHoi.MaNhom = maxgr;
                        db.NhomCauHois.Add(nhomCauHoi);
                        nhomcauhoilast = nhomCauHoi;
                        newGroup = false;
                        fi = false;
                    }

                    if (parts[4].Contains("\n") && !Regex.IsMatch(parts[4], @"Question\s\d+"))
                    {
                        newGroupContent = parts[4].Substring(parts[4].IndexOf("\n")).Trim();
                        if (!string.IsNullOrEmpty(newGroupContent))
                        {
                            newGroup = true;
                        }
                        parts[4] = parts[4].Substring(0, parts[4].IndexOf("\n")).Trim();
                    }

                    var cauHoi = new CauHoi
                    {
                        MaCauHoi = ++maxMaCauHoi,
                        NoiDung = parts[0].Trim(),
                        DapAnA = parts[1].Substring(2).Trim(),
                        DapAnB = parts[2].Substring(2).Trim(),
                        DapAnC = parts[3].Substring(2).Trim(),
                        DapAnD = parts[4].Substring(2).Trim(),
                        MaNhom = maxgr,
                        DapAnChinhXac = "A"
                    };
                    cauHoi.KyThis.Add(kt);

                    db.CauHois.Add(cauHoi);
                    cauhois123.Add(cauHoi);

                    if (newGroupContent.ToLower().Contains("đáp án"))
                    {
                        break;
                    }
                }

                if (newGroupContent.ToLower().Contains("đáp án"))
                {
                    var answerPattern = new Regex(@"\d+\.\s*[A-D]", RegexOptions.Compiled);
                    var answerMatches = answerPattern.Matches(newGroupContent);

                    foreach (Match answerMatch in answerMatches)
                    {
                        var answerParts = answerMatch.Value.Split('.');
                        if (answerParts.Length == 2)
                        {
                            if (int.TryParse(answerParts[0].Trim(), out int questionIndex) && questionIndex <= cauhois123.Count)
                            {
                                cauhois123[questionIndex - 1].DapAnChinhXac = answerParts[1].Trim(new char[] { ' ', '\n', '\r', '\t' });
                            }
                        }
                    }
                }
                if (kt.SoCauHoi == null || kt.SoCauHoi <= 0) kt.SoCauHoi = kt.CauHois.Count;

                db.SaveChanges();
            }
        }
        public JsonResult UploadAudio(HttpPostedFileBase audioFile)
        {
            if (audioFile != null && audioFile.ContentLength > 0)
            {
                // Specify the path where you want to save the file
                string folderPath = Server.MapPath("~/UploadedAudios/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);  // Create directory if it doesn't exist
                }

                // Generate a unique file name to avoid overwriting
                string fileName = Path.GetFileName(audioFile.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                audioFile.SaveAs(filePath);  // Save the file to the server

                // Return the file URL in the JSON response
                return Json(new
                {
                    location = Url.Content("~/UploadedAudios/" + fileName)  // Return the URL for the audio file
                });
            }

            return Json(new { error = "No file uploaded or file is empty." });
        }






        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LuuThayDoi(int made, string action, FormCollection f)
        {
            if (action.StartsWith("delete_"))
            {
                int maCauHoi = int.Parse(action.Split('_')[1]);
                return DeleteCauHoi(maCauHoi, made);
            }
            else
            {
                using (var db = new TracNghiemTiengAnhTHPTEntities1())
                {
                    List<NhomCauHoi> a = db.NhomCauHois.ToList();

                    foreach (var item in a)
                    {
                        string q = "NoiDung_Nhom" + item.MaNhom;

                        if (!string.IsNullOrEmpty(f[q]))
                        {
                            item.NoiDung = f[q];
                        }
                        foreach (var item1 in item.CauHois)
                        {
                            string radioQuestionKey = "radio_answer_" + item1.MaCauHoi;
                            string checkboxQuestionKey = "checkbox_answer_" + item1.MaCauHoi;

                            if (!string.IsNullOrEmpty(f[radioQuestionKey]) && f["questionType_" + item1.MaCauHoi] == "radio")
                            {
                                item1.DapAnChinhXac = f[radioQuestionKey];
                            }
                            else if (f.GetValues(checkboxQuestionKey) != null && f["questionType_" + item1.MaCauHoi] == "checkbox")
                            {
                                var selectedAnswers = f.GetValues(checkboxQuestionKey);
                                item1.DapAnChinhXac = string.Join("", selectedAnswers);
                            }


                            string questionKey = "NoiDung_CauHoi" + item1.MaCauHoi;
                            if (!string.IsNullOrEmpty(f[questionKey]))
                            {
                                // Encode the HTML content before saving
                                item1.NoiDung = HttpUtility.HtmlEncode(f[questionKey]);
                            }
                        }
                    }

                    KyThi kythi = db.KyThis.FirstOrDefault(k => k.MaDe == made);
                    if (kythi != null)
                    {
                        kythi.ThoiGian = int.Parse(f["KyThi[" + made + "].ThoiGian"]);
                        kythi.SoCauHoi = int.Parse(f["KyThi[" + made + "].SoCauHoi"]);

                        // Retrieve the selected values from the dropdown lists
                        string selectedClass = f["Class"];
                        string selectedExamType = f["ExamType"];
                        foreach (CauHoi cauhoi in kythi.CauHois)
                        {
                            if (!string.IsNullOrEmpty(selectedClass))
                            {

                                var dangbai = db.DangBais.FirstOrDefault(d => d.TenLoai == selectedClass);
                                if (dangbai != null)
                                {
                                    var itemsToRemove = cauhoi.DangBais.Where(n => n.TenLoai.Contains("Lớp")).ToList();
                                    foreach (var item in itemsToRemove)
                                    {
                                        cauhoi.DangBais.Remove(item);
                                    }
                                    cauhoi.DangBais.Add(dangbai);
                                }
                            }
                            if (!string.IsNullOrEmpty(selectedExamType))
                            {
                                var dangbai = db.DangBais.FirstOrDefault(d => d.TenLoai == selectedExamType);
                                if (dangbai != null)
                                {
                                    var itemsToRemove = cauhoi.DangBais.Where(c => c.TenLoai == "Friends Global" || c.TenLoai == "Global Success" || c.TenLoai == "Smart World").ToList();
                                    foreach (var item in itemsToRemove)
                                    {
                                        cauhoi.DangBais.Remove(item);
                                    }
                                    cauhoi.DangBais.Add(dangbai);
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("ChiTietDeThi", new { made });
            }
        }


        public ActionResult AutoGenerateExam()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var totalQuestions = db.CauHois.Count();
                var easyQuestions = db.CauHois.Count(q => q.MucDo == 1);
                var hardQuestions = db.CauHois.Count(q => q.MucDo == 2);

                ViewBag.TotalQuestions = totalQuestions;
                ViewBag.EasyQuestions = easyQuestions;
                ViewBag.HardQuestions = hardQuestions;
            }

            return View();
        }
        [HttpPost]
        public ActionResult AutoGenerateExam(int SoCauHoiDe, int SoCauHoiKho, int ThoiGian)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var easyQuestions = db.CauHois
                    .Where(q => q.MucDo == 1 && !q.isDeleted)
                    .ToList();
                var hardQuestions = db.CauHois
                    .Where(q => q.MucDo == 2 && !q.isDeleted)
                    .ToList();

                if (SoCauHoiDe > easyQuestions.Count || SoCauHoiKho > hardQuestions.Count)
                {
                    ModelState.AddModelError("", "Không đủ câu hỏi để tạo đề thi.");
                    TempData["Error"] = "Không đủ câu hỏi để tạo đề thi.";
                    ViewBag.TotalQuestions = db.CauHois.Count();
                    ViewBag.EasyQuestions = easyQuestions.Count;
                    ViewBag.HardQuestions = hardQuestions.Count;
                    return View();
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
                    TenKyThi = "Đề Thi Ngẫu Nhiên " + DateTime.Now.ToString("yyyyMMddHHmm") + " - " + Guid.NewGuid().ToString().Substring(0, 8),
                    ThoiGian = ThoiGian,
                    ThoiGianBatDau = DateTime.Now,
                    isDeleted = false,
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
                return RedirectToAction("Index");
            }
        }



    }






}