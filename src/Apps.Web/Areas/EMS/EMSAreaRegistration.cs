using System.Web.Mvc;

namespace Apps.Web.Areas.EMS
{
    public class EMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
               "EMSGlobalization", // 路由名称
               "{lang}/EMS/{controller}/{action}/{id}", // 带有参数的 URL
               new { lang = "zh", controller = "Home", action = "Index", id = UrlParameter.Optional }, // 参数默认值
               new { lang = "^[a-zA-Z]{2}(-[a-zA-Z]{2})?$" }    //参数约束
           );
            context.MapRoute(
                "EMS_default",
                "EMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}