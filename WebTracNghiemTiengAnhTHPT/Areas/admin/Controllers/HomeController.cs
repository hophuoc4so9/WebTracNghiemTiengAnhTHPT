using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: admin/Home
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult QL_LOPHOC()
        {
            return View();
        }

        //edit user 
        [HttpPost]
        public JsonResult EditUser(string Username,string PhanQuyen, string Gmail)
        {
            try
            {
                using (TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1())
                {
                    var user = db.TaiKhoans.FirstOrDefault(u => u.Username == Username);

                    if (user != null)
                    {
                        user.Username = Username;
                        user.PhanQuyen = PhanQuyen;
                        user.Gmail = Gmail;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Bạn đã cập nhật user này thành công!";
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred while deleting the user." });
            }
        }
        //delete user
        [HttpPost]
        public JsonResult DeleteUser(string Username)
        {
            try
            {
                using (TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1())
                {
                    var user = db.TaiKhoans.FirstOrDefault(u => u.Username == Username);  

                    if (user != null)
                    {
                        user.isDeleted = true;
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Bạn đã xóa user này thành công. User này sẽ được chuyển vào mục thùng rác !";
                        return Json(new { success = true});
                    }
                    else
                    {
                        return Json(new { success = false});
                    }
                }
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = "An error occurred while deleting the user." });
            }
        }

        [HttpPost]
        public JsonResult ChangeStatus(string Username, bool isActive)
        {
            try
            {
                var gg = Username; 
                TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
                List<TaiKhoan> model = db.TaiKhoans.ToList();
                // Assuming you have a DbContext for accessing the database
                var user = model.SingleOrDefault(u => u.Username == gg);
                if (user != null)
                {
                    user.status = isActive;  
                    db.SaveChanges(); 
                }
                TempData["SuccessMessage"] = "Bạn đã thay đổi trạng thái thành công.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Render_QLLOPHOC()
        {

            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<TaiKhoan> model = db.TaiKhoans.ToList();
            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult RenderNavbar()
        {
            return PartialView("_Navbar");
        }
        public ActionResult RenderMain()
        {

            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<TaiKhoan> model = db.TaiKhoans.ToList();
            return PartialView(model);
        }

        public ActionResult PartialPhanQuyen()
        {
            if(Session["phanquyen"]==null)
            {
                return null;
            }    

            if (Session["phanquyen"].ToString() == "admin")
             {
                return PartialView("_PartialChucNangAdmin");
            }
            else
            {
                return PartialView("~/Areas/giaovien/Views/Shared/_PartialChucNangGiaoVien.cshtml");
            }    
        }
        // GET: Admin/TaiKhoan/AddNew
        public ActionResult AddNew()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Fetch distinct roles for the dropdown
                var roles = db.TaiKhoans
                              .Select(r => r.PhanQuyen)
                              .Distinct()
                              .Select(r => new SelectListItem
                              {
                                  Value = r,
                                  Text = r
                              })
                              .ToList();
                ViewBag.Roles = roles;
            }
            return View();
        }

        // POST: Admin/TaiKhoan/AddNew
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new TracNghiemTiengAnhTHPTEntities1())
                {
                    model.isDeleted = false; 
                    db.TaiKhoans.Add(model);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm mới 1 tài khoản thành công!";
                }

                return RedirectToAction("Index");
            }
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var roles = db.TaiKhoans
                              .Select(r => r.PhanQuyen)
                              .Distinct()
                              .Select(r => new SelectListItem
                              {
                                  Value = r,
                                  Text = r
                              })
                              .ToList();
                ViewBag.Roles = roles;
            }

            return View(model);
        }

    }
}