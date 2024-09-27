using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class SignUpController : Controller
    {
        // GET: admin/SignUp
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(string name, string email, string password, string repassword)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(repassword))
            {
                ViewBag.Message = "Tất cả các trường là bắt buộc.";
                return View();
            }

         
            if (!IsValidEmail(email))
            {
                ViewBag.Message = "Email không hợp lệ.";
                return View();
            }

      
            if (password != repassword)
            {
                ViewBag.Message = "Mật khẩu xác nhận không khớp.";
                return View();
            }

         
            if (password.Length < 6)
            {
                ViewBag.Message = "Mật khẩu phải có ít nhất 6 ký tự.";
                return View();
            }
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var existingUser = db.TaiKhoans.FirstOrDefault(u => u.Gmail == email);
                if (existingUser != null)
                {
                    ViewBag.Message = "Email đã được sử dụng.";
                    return View();
                }
                var newTaiKhoan = new TaiKhoan
                {
                    Username = name,
                    Password = HashPassword(password),
                    PhanQuyen = "hocsinh",
                    Gmail = email,
                   

                };
                var test = HashPassword(password).Length;
                try
                {
                    db.TaiKhoans.Add(newTaiKhoan);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            // Optionally log this to a file or a logging system
                        }
                    }

                    ViewBag.Message = "Đã xảy ra lỗi khi đăng ký tài khoản. Vui lòng kiểm tra lại thông tin.";
                    return View();
                }

                Session["UserName"] = name;
                Session["phanquyen"] = "hocsinh";
                // Set success message
                TempData["SuccessMessage"] = "Bạn đã đăng ký tài khoản thành công.";
                return RedirectToAction("Login", "Login");

            }
           
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

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}