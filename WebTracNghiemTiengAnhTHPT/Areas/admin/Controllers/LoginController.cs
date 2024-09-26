using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebTracNghiemTiengAnhTHPT.Models;
using static System.Collections.Specialized.BitVector32;

namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: admin/Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                var user = db.TaiKhoans.SingleOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                   var PhanQuyen = user.PhanQuyen; 
                
                    if(PhanQuyen != null)
                    {
                        FormsAuthentication.SetAuthCookie(username, false);
                       
                        Session["UserName"] = username;
                        Session["phanquyen"] = PhanQuyen;
                        switch (PhanQuyen)
                        {
                          
                            case "admin":
                                return RedirectToAction("Index", "Home");
                              
                            case "giaovien":
                                return RedirectToAction("Index", "Home", new { area = "giaovien" });
                              
                            case "hocsinh":                        
                                return RedirectToAction("Index", "Contests", new { area = "" });
                               
                        }
                    }
                   
                    
                }
                else
                {
     
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View();
        }

    }
}