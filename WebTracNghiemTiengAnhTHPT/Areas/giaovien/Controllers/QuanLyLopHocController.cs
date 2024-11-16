using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyLopHocController : Controller
    {
        // GET: giaovien/QuanLyLopHoc
        public ActionResult Index()
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<LopHoc> model = context.LopHocs.ToList();
                return View(model);
            }
        }
        public ActionResult Create()
        {

            return View();
        }
        // POST: PhongThi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LopHoc viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {

                    context.LopHocs.Add(viewModel);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(viewModel);
        }
        public ActionResult Details(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                LopHoc lophoc = context.LopHocs.Find(id);
                if (lophoc == null)
                {
                    return HttpNotFound();
                }
                return View(lophoc);
            }
        }
        public ActionResult Edit(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {

                LopHoc lophoc = context.LopHocs.Find(id);
                Session["malop"] = id;

                if (lophoc == null)
                {
                    return HttpNotFound();
                }



                return View(lophoc);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LopHoc viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TracNghiemTiengAnhTHPTEntities1())
                {

                    int malop = (int)Session["malop"];
                    var currentLop = context.LopHocs.FirstOrDefault(k => k.MaLop == malop);

                    if (currentLop != null)
                    {

                        currentLop.TenLop = viewModel.TenLop;

                        context.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(viewModel);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Tìm lớp học theo ID
                LopHoc lophoc = context.LopHocs.Find(id);

                if (lophoc == null)
                {
                    return HttpNotFound();
                }

                // Xóa lớp học
                context.LopHocs.Remove(lophoc);
                context.SaveChanges();

                // Trả về JSON cho Ajax
                return Json(new { success = true });
            }
        }


    }
}