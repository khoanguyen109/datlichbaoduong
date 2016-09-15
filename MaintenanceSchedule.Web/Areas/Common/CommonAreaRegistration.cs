using System.Web.Mvc;

namespace MaintenanceSchedule.Web.Areas.Common
{
    public class CommonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Common_default",
                "Common/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Common.Controllers" }
            );

            context.MapRoute(
                "SignIn",
                "admin/dang-nhap.html",
                new { action = "SignIn", Controller = "Account" },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Common.Controllers" }
            );
        }
    }
}
