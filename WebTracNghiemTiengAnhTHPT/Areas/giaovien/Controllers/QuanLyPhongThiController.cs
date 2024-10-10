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
        // GET: giaovien/QuanLyPhongThi
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<PhongThi> model = context.PhongThis.ToList();
                return View(model);
            }
        }
        // GET: PhongThi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhongThi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhongThi phongThi)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {

                    phongThi.MaPhong = PhongThi.GenerateMaPhong(1000);  // Tạo mã phòng ngẫu nhiên
                    context.PhongThis.Add(phongThi);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(phongThi);
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
                PhongThi phongThi = context.PhongThis.Find(id);
                Session["maphong"] = id;
                if (phongThi == null)
                {
                    return HttpNotFound();
                }
                return View(phongThi);
            }
        }
        // POST: PhongThi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhongThi phongThi)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {
                    int maphong = (int)Session["maphong"];
                    var currentPhong = context.PhongThis.FirstOrDefault(k => k.MaPhong == maphong);

                    if (currentPhong != null)
                    {

                        currentPhong.TenPhong = phongThi.TenPhong;
                        currentPhong.MatKhau = phongThi.MatKhau;


                        context.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }


            }
            return View(phongThi);
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