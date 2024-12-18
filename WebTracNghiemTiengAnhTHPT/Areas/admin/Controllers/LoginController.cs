﻿using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: admin/Login
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["phanquyen"] = null;
            return View("Login");
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var Hashpassword = HashPassword(password);
                var user = db.TaiKhoans.SingleOrDefault(u => u.Username == username && (u.Password == password || u.Password == Hashpassword));
                if (user != null)
                {
                    var PhanQuyen = user.PhanQuyen;

                    if (PhanQuyen != null)
                    {
                        FormsAuthentication.SetAuthCookie(username, false);

                        Session["UserName"] = username;
                        Session["phanquyen"] = PhanQuyen;
                        switch (PhanQuyen)
                        {

                            case "admin":
                                TempData["SuccessMessage"] = $"Chào {username}, bạn đã đăng nhập thành công thành công.";

                                return RedirectToAction("Index", "Home");

                            case "giaovien":
                                TempData["SuccessMessage"] = $"Chào {username}, bạn đã đăng nhập thành công thành công.";
                                return RedirectToAction("Index", "Home", new { area = "giaovien" });

                            case "hocsinh":
                                TempData["SuccessMessage"] = $"Chào {username}, bạn đã đăng nhập thành công thành công.";
                                return RedirectToAction("Index", "Contests", new { area = "" });
                        }
                    }

                }
                else
                {


                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            ViewBag.Message = "Tài Khoản không tồn tại";
            return View();
        }
        private string HashPassword(string password)
        {

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            using (var sha256 = SHA256.Create())
            {

                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                var base64Hash = Convert.ToBase64String(hashedBytes);


                return base64Hash.Substring(0, Math.Min(28, base64Hash.Length));
            }
        }

    }
}