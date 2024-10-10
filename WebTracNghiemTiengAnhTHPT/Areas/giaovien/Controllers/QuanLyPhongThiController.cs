using iText.Forms.Xfdf;
using System;
using System.Collections.Generic;
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
                // Lấy danh sách PhongThi và bao gồm thông tin từ bảng LopHoc
                var phongThiList = context.PhongThis.Include("LopHoc").ToList();

                // Tạo danh sách PhongThiViewModel
                var viewModelList = phongThiList.Select(pt => new PhongThiViewModel
                {
                    PhongThi = pt,
                    LopHocList = pt.LopHoc != null
                        ? new List<SelectListItem>
                        {
                    new SelectListItem { Value = pt.LopHoc.MaLop.ToString(), Text = pt.LopHoc.TenLop }
                        }
                        : new List<SelectListItem>() // Trả về danh sách rỗng nếu LopHoc là null
                }).ToList();

                return View(viewModelList);
            }
        }



        // GET: PhongThi/Create
        public ActionResult Create()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Truy vấn danh sách lớp học từ bảng LopHoc
                var lopHocList = context.LopHocs.Select(l => new SelectListItem
                {
                    Value = l.MaLop.ToString(),
                    Text = l.TenLop
                }).ToList();

                // Tạo ViewModel và gán danh sách LopHocList
                var viewModel = new PhongThiViewModel
                {
                    LopHocList = lopHocList,
                    PhongThi = new PhongThi() // Khởi tạo đối tượng PhongThi
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
                    // Lưu phòng thi với MaLop được chọn từ dropdown
                    context.PhongThis.Add(viewModel.PhongThi);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            // Nếu có lỗi, tải lại danh sách LopHocList
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
                PhongThi phongThi = context.PhongThis.Find(id);
                if (phongThi == null)
                {
                    return HttpNotFound();
                }
                return View(phongThi);
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


    }
}