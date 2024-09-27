using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyDeThiController : Controller
    {
        // GET: giaovien/QuanLyDeThi
        public ActionResult Index()
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                List<KyThi> model = db.KyThis.ToList();
                //model=model.Where(item => item.UsernameTacGia == Session["UserName"].ToString()).ToList();  
                return View(model);
            }

           
        }
        [HttpPost]
        public ActionResult Update(FormCollection form)
        {
            using (var db = new TracNghiemTiengAnhTHPTEntities1())
            {
                // Loop through the KyThi entries
                foreach (var key in form.AllKeys)
                {
                    if (key.StartsWith("KyThi["))
                    {
                        // Extract the index
                        var indexStart = key.IndexOf("[") + 1;
                        var indexEnd = key.IndexOf("]", indexStart);
                        var indexString = key.Substring(indexStart, indexEnd - indexStart);
                        int index;

                        if (int.TryParse(indexString, out index))
                        {
                            // Get the MaDe value to find the specific KyThi item
                            var maDe = form[$"KyThi[{index}].MaDe"];
                            var kyThi = db.KyThis.Find(maDe);

                            if (kyThi != null)
                            {
                                // Update the fields
                                kyThi.TenKyThi = form[$"KyThi[{index}].TenKyThi"];
                                kyThi.ThoiGian = int.TryParse(form[$"KyThi[{index}].ThoiGian"], out var thoiGian) ? thoiGian : 0;

                                // Update ThoiGianBatDau
                                var batDauDate = form[$"KyThi[{index}].ThoiGianBatDauDate"];
                                var batDauTime = form[$"KyThi[{index}].ThoiGianBatDauTime"];
                                kyThi.ThoiGianBatDau = string.IsNullOrEmpty(batDauDate) || string.IsNullOrEmpty(batDauTime)
                                    ? (DateTime?)null
                                    : DateTime.Parse($"{batDauDate} {batDauTime}");

                                // Update ThoiGianKetThuc
                                var ketThucDate = form[$"KyThi[{index}].ThoiGianKetThucDate"];
                                var ketThucTime = form[$"KyThi[{index}].ThoiGianKetThucTime"];
                                kyThi.ThoiGianKetThuc = string.IsNullOrEmpty(ketThucDate) || string.IsNullOrEmpty(ketThucTime)
                                    ? (DateTime?)null
                                    : DateTime.Parse($"{ketThucDate} {ketThucTime}");

                                // Update CongKhai
                                kyThi.CongKhai = form[$"KyThi[{index}].CongKhai"] == "true";

                                // Handle isDeleted
                                kyThi.isDeleted = form[$"KyThi[{index}].isDeleted"] == "true"; // Set as deleted if true
                            }
                        }
                    }
                }

                // Save changes to the database
                db.SaveChanges();
            }

            // Redirect or return a view as appropriate
            return RedirectToAction("Index");
        }



    }
}