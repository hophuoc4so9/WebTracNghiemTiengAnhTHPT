using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.hocsinh.Controllers
{
    public class ThongTinCaNhanController : Controller
    {
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
        // GET: hocsinh/ThongTinCaNhan
        public ActionResult Index()
        {
            var userName = Session["UserName"] as string;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }

            TaiKhoan userInfo = GetUserInfoByUserName(userName);

            if (userInfo == null)
            {
                // Thêm log để kiểm tra
                System.Diagnostics.Debug.WriteLine("Không tìm thấy người dùng.");
                return HttpNotFound();
            }

            // Thêm log để kiểm tra mô hình
            System.Diagnostics.Debug.WriteLine($"Thông tin người dùng: {userInfo.HoTen}");

            return View(userInfo);
        }
        public ActionResult ThongTinCaNhan()
        {
            var userName = Session["UserName"] as string;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Admin");
            }

            TaiKhoan userInfo = GetUserInfoByUserName(userName);

            if (userInfo == null)
            {
                // Thêm log để kiểm tra
                System.Diagnostics.Debug.WriteLine("Không tìm thấy người dùng.");
                return HttpNotFound();
            }

            // Thêm log để kiểm tra mô hình
            System.Diagnostics.Debug.WriteLine($"Thông tin người dùng: {userInfo.HoTen}");

            return View(userInfo);
        }
        private TaiKhoan GetUserInfoByUserName(string userName)
        {
                return db.TaiKhoans.FirstOrDefault(u => u.Username == userName); // Thay đổi tên bảng và cột theo cơ sở dữ liệu của bạn
        }
    } 
}