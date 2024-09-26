using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;
namespace WebTracNghiemTiengAnhTHPT.Controllers
{
    public class ContestsController : Controller
    {
        // GET: Contests
        public ActionResult Index()
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<view_ChitietKyThi> model = db.view_ChitietKyThi.ToList();
            return View(model);
            
        }
       
        public ActionResult ChiTietKyThi(string made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
            List<CauHoi> model = db.CauHois.ToList();
            model = model.Where(c => c.KyThis.Any(kc => kc.MaDe == made)).ToList();
            ViewBag.MaDe = made;
            if (Session["UserName"] == null)
            {

                return RedirectToAction("Login", "Login", new { area = "admin" });
            }
            else
            return View(model);

        }
        [HttpPost]
        public ActionResult Result(FormCollection form, string made)
        {
            TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();


            List<ChiTietKetQua> results = new List<ChiTietKetQua>();
            // Duyệt qua từng câu hỏi

            foreach (var item in db.CauHois.ToList())
            {
                string questionKey = "answer_" + item.MaCauHoi;
                string selectedValue = form[questionKey];

                if (!string.IsNullOrEmpty(selectedValue))
                {
                    ChiTietKetQua tam = new ChiTietKetQua();
                    tam.MaCauHoi=item.MaCauHoi;
                    tam.DapAnChon=  selectedValue;
                    if(Session["UserName"]!=null)
                    tam.Username = Session["UserName"].ToString();
                    tam.MaDe = made;
                   results.Add(tam);
                }
            }
         
            // Xử lý kết quả, ví dụ: lưu vào database hoặc hiển thị ra
            foreach (var result in results)
            {
                // result.MaCauHoi và result.SelectedValue chứa giá trị cần thiết
              
            }
            return View(results);
        }


    }
}