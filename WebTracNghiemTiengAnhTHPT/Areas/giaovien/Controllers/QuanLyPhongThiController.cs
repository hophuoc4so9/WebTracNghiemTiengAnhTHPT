using iText.Forms.Xfdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyPhongThiController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
            
                var phongThiList = context.PhongThis.Include("LopHoc").ToList();

           
                var viewModelList = phongThiList.Select(pt => new PhongThiViewModel
                {
                    PhongThi = pt,
                    LopHocList = pt.LopHoc != null
                        ? new List<SelectListItem>
                        {
                    new SelectListItem { Value = pt.LopHoc.MaLop.ToString(), Text = pt.LopHoc.TenLop }
                        }
                        : new List<SelectListItem>() 
                }).ToList();

                return View(viewModelList);
            }
        }



        // GET: PhongThi/Create
        public ActionResult Create()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
             
                var lopHocList = context.LopHocs.Select(l => new SelectListItem
                {
                    Value = l.MaLop.ToString(),
                    Text = l.TenLop
                }).ToList();

             
                var viewModel = new PhongThiViewModel
                {
                    LopHocList = lopHocList,
                    PhongThi = new PhongThi() 
                };

                return View(viewModel);
            }
        }


        // POST: PhongThi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhongThiViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {
                   
                    context.PhongThis.Add(viewModel.PhongThi);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                viewModel.LopHocList = context.LopHocs.Select(l => new SelectListItem
                {
                    Value = l.MaLop.ToString(),
                    Text = l.TenLop
                }).ToList();
            }

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                PhongThi phongThi = context.PhongThis.Include("KyThis").FirstOrDefault(pt => pt.MaPhong == id);

                string usernameTacGia = Session["UserName"].ToString();

                ViewBag.otherKyThi = context.KyThis
                    .Where(k => k.PhongThis.All(n => n.MaPhong != id) && k.UsernameTacGia == usernameTacGia && k.ThoiGianKetThuc> DateTime.Now)
                    .ToList();

                if (phongThi == null)
                {
                    return HttpNotFound();
                }

                return View(phongThi);
            }
        }

        [HttpPost]
        public ActionResult AddKyThiToPhongThi(int MaPhong, int KyThiId)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                var phongThi = context.PhongThis.Include("KyThis").FirstOrDefault(pt => pt.MaPhong == MaPhong);
                var kyThi = context.KyThis.Find(KyThiId);

                if (phongThi != null && kyThi != null)
                {
                    phongThi.KyThis.Add(kyThi);
                    context.SaveChanges();
                }

                return RedirectToAction("Details", new { id = MaPhong });
            }
        }
        // GET: PhongThi/Edit/5
        public ActionResult Edit(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Lấy thông tin phòng thi theo id
                PhongThi phongThi = context.PhongThis.Find(id);
                Session["maphong"] = id;  // Lưu id phòng thi vào Session

                if (phongThi == null)
                {
                    return HttpNotFound();
                }

                // Truy vấn danh sách lớp học từ bảng LopHoc
                var lopHocList = context.LopHocs.Select(l => new SelectListItem
                {
                    Value = l.MaLop.ToString(),
                    Text = l.TenLop
                }).ToList();

                // Tạo ViewModel và gán dữ liệu
                var viewModel = new PhongThiViewModel
                {
                    PhongThi = phongThi,
                    LopHocList = lopHocList
                };

                return View(viewModel);
            }
        }

        // POST: PhongThi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhongThiViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {
                    // Lấy id phòng thi từ Session
                    int maphong = (int)Session["maphong"];
                    var currentPhong = context.PhongThis.FirstOrDefault(k => k.MaPhong == maphong);

                    if (currentPhong != null)
                    {
                        // Cập nhật các thuộc tính từ ViewModel
                        currentPhong.TenPhong = viewModel.PhongThi.TenPhong;
                        currentPhong.MatKhau = viewModel.PhongThi.MatKhau;
                        currentPhong.malop = viewModel.PhongThi.malop;  // Cập nhật MaLop từ dropdown

                        context.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }

            // Nếu có lỗi, truy vấn lại danh sách LopHocList
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                viewModel.LopHocList = context.LopHocs.Select(l => new SelectListItem
                {
                    Value = l.MaLop.ToString(),
                    Text = l.TenLop
                }).ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Tìm phòng thi theo ID
                PhongThi phongThi = context.PhongThis.Find(id);

                if (phongThi == null)
                {
                    return HttpNotFound();
                }

                // Xóa phòng thi
                context.PhongThis.Remove(phongThi);
                context.SaveChanges();

                // Trả về JSON cho Ajax
                return Json(new { success = true });
            }
        }
        public ActionResult ExportToExcel(int phongThiId)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            var phongThi = db.PhongThis.Find(phongThiId); // Fetch the exam room details from the database
            if (phongThi == null)
            {
                return HttpNotFound();
            }

            var kyThis = phongThi.KyThis.ToList(); // Fetch the exams associated with the room

            // Step 1: Create Excel file and insert data
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Results");

                // Add headers
                worksheet.Cells[1, 1].Value = "Username";
                worksheet.Cells[1, 2].Value = "Họ và tên";
                int j = 3;
                for (j = 3; j < kyThis.Count + 3; j++)
                {
                    worksheet.Cells[1, j].Value = kyThis[j - 3].TenKyThi;
                }
                var hocSinhs = phongThi.TaiKhoans.ToList();
                for (int i = 0; i < hocSinhs.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = hocSinhs[i].Username;
                    worksheet.Cells[i + 2, 2].Value = hocSinhs[i].HoTen;
                    for (j = 3; j < kyThis.Count + 3; j++)
                    {
                        var diem = kyThis[j - 3].KetQuas.FirstOrDefault(kq => kq.Username == hocSinhs[i].Username)?.Diem;
                        if (diem != null)
                        {
                            worksheet.Cells[i + 2, j].Value = diem;
                        }
                    }
                }

                // Save the file to a memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Return the file as a download
                string fileName = $"PhongThi_{phongThi.MaPhong}_Results.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


    }
}