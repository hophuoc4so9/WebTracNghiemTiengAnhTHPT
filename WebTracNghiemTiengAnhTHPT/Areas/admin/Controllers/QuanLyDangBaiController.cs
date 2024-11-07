using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTracNghiemTiengAnhTHPT.Models;
namespace WebTracNghiemTiengAnhTHPT.Areas.admin.Controllers
{
    public class QuanLyDangBaiController : Controller
    {
        // GET: admin/QuanLyDangBai

        // GET: admin/ChuDe
        TracNghiemTiengAnhTHPTEntities1 db = new TracNghiemTiengAnhTHPTEntities1();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult DsChuDe(int page = 1, int pageSize = 5, string search = "")
        {
            try
            {
                var dsCD = (from cd in db.DangBais
                            where cd.TenLoai.Contains(search)
                            select new
                            {
                                MaCD = cd.MaLoai,
                                TenCD = cd.TenLoai
                            }).ToList();

                var totalItems = dsCD.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var paginatedDsCD = dsCD.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Json(new
                {
                    code = 200,
                    dsCD = paginatedDsCD,
                    totalItems = totalItems,
                    totalPages = totalPages,
                    currentPage = page,
                    msg = "Lấy danh sách dạng bài thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách dạng bài thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult Detail(int maCD)
        {
            try
            {
                var cd = (from s in db.DangBais
                          where (s.MaLoai == maCD)
                          select new
                          {
                              MaCD = s.MaLoai,
                              TenChuDe = s.TenLoai
                          }).SingleOrDefault();
                //db.CHUDEs.SingleOrDefault(c => c.MaCD == maCD);
                return Json(new { code = 200, cd = cd, msg = "Lấy thông tin dạng bài thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg
                =
                "Lấy thông tin dạng bài thất bại." + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddChuDe(string strTenCD)
        {
            try
            {
                var cd = new DangBai();
                cd.TenLoai = strTenCD; db.DangBais.Add(cd); db.SaveChanges();
                return Json(new { code = 200, msg = "Thêm dạng bài thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thêm dạng bài thất bại. Lỗi " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(int maCD, string strTenCD)
        {
            try
            {
                var cd = db.DangBais.SingleOrDefault(c => c.MaLoai == maCD); 
                cd.TenLoai = strTenCD;
                db.SaveChanges();
                return Json(new { code = 200, msg = "Sửa dạng bài thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg = "Sửa dạng bài thất bại. Lỗi" + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete(int maCD)
        {
            try
            {

                var cd = db.DangBais.SingleOrDefault(c => c.MaLoai == maCD);
                db.DangBais.Remove(cd);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa dạng bài thành công." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    code = 500,
                    msg = "Xóa dạng bài thất bại. Lỗi" + ex.Message
                }, JsonRequestBehavior.AllowGet);

            }
        }

    }
}