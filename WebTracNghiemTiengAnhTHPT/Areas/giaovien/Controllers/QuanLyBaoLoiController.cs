using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
            if (MaCauHoi == null || string.IsNullOrEmpty(ResponseText))
            {
                return Json(new { success = false, message = "Mã câu hỏi và nội dung phản hồi không được để trống." });
            }


            var errorReport = _db.BaoLois.SingleOrDefault(k => k.MaCauHoi == MaCauHoi);
            if (errorReport != null)
            {
                errorReport.Response = ResponseText;
                _db.SaveChanges();



                return Json(new { success = true, message = "Phản hồi đã được gửi thành công!" });
            }

            return Json(new { success = false, message = "Không tìm thấy báo lỗi với mã câu hỏi đã cho." });
        }
    }
}
