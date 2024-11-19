using System.Web.Mvc;

namespace WebTracNghiemTiengAnhTHPT.Areas.giaovien
{
    public class giaovienAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "giaovien";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "giaovien_default",
                "giaovien/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}