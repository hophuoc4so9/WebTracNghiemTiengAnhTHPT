﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                        switch (PhanQuyen)
                        {
                            case "admin":
                                return RedirectToAction("Index", "Home");
                                break;
                            case "giavien":
                                return RedirectToAction("Index", "Contests");
                                break;
                            case "hocsinh":
                                    return RedirectToAction("Index", "Contests");
                                break;
                        }
                    }
                    FormsAuthentication.SetAuthCookie(username, false);
                    
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