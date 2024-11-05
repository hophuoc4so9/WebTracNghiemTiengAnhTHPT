using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using WebTracNghiemTiengAnhTHPT.Models;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien.Controllers
{
    public class QuanLyBaoLoiController : Controller
    {
        private TracNghiemTiengAnhTHPTEntities1 _db = new TracNghiemTiengAnhTHPTEntities1();

        // GET: giaovien/QuanLyBaoLoi
        public ActionResult Index()
        {
            string username = Session["UserName"]?.ToString();
            var errors = _db.BaoLois
                .Include(b => b.KyThi)
                .Where(b => b.KyThi.UsernameTacGia == username)
                .ToList();

            return View(errors);
        }

        // POST: giaovien/QuanLyBaoLoi/RespondToError
        [HttpPost]
        public JsonResult RespondToError(int MaCauHoi, string ResponseText)
        {
            if (MaCauHoi==null || string.IsNullOrEmpty(ResponseText))
            {
                return Json(new { success = false, message = "Mã câu hỏi và nội dung phản hồi không được để trống." });
            }

            // Find the BaoLoi record to update
            var errorReport = _db.BaoLois.SingleOrDefault(k=>k.MaCauHoi==MaCauHoi);
            if (errorReport != null)
            {
                if (string.IsNullOrEmpty(errorReport.Response))
                {
                    errorReport.Response = ResponseText; // Update the Response field
                    _db.SaveChanges(); // Save changes to the database
                }
                else
                {
                    BaoLoi newBaoloi = new BaoLoi();
                    newBaoloi.NoiDung = errorReport.NoiDung;
                    newBaoloi.MaCauHoi = errorReport.MaCauHoi;
                    newBaoloi.MaDe = errorReport.MaDe;
                    newBaoloi.Username = errorReport.Username;
                    newBaoloi.Response = ResponseText;
                    _db.BaoLois.Add(newBaoloi);
                    _db.SaveChanges(); 
                }

                return Json(new { success = true, message = "Phản hồi đã được gửi thành công!" });
            }

            return Json(new { success = false, message = "Không tìm thấy báo lỗi với mã câu hỏi đã cho." });
        }
    }
}
