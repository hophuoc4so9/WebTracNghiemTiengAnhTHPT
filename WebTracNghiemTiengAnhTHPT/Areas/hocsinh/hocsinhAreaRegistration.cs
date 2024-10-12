using System.Web.Mvc;

namespace WebTracNghiemTiengAnhTHPT.Areas.hocsinh
{
    public class hocsinhAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "hocsinh";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "hocsinh_default",
                "hocsinh/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}