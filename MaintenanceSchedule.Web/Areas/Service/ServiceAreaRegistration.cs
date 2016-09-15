using System.Web.Mvc;

namespace MaintenanceSchedule.Web.Areas.Service
{
    public class ServiceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Service";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            var serviceCommonPrefix = "dich-vu";

            context.MapRoute(
                "Service_Default",
                "Service/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Service.Controllers" }
            );

            context.MapRoute(
                "Service_Index",
                serviceCommonPrefix + ".html",
                new
                {
                    action = "Index",
                    controller = "Service",
                    AreaName = "Service"
                },
                //"Service/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Service.Controllers" }
            );

            context.MapRoute(
                "Service_Detail",
                serviceCommonPrefix + "/{title}-{id}.html",                
                new
                {
                    action = "View",
                    controller = "Service",
                    AreaName = "Service",
                    id = UrlParameter.Optional,
                    title = UrlParameter.Optional
                },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Service.Controllers" }
            );
        }
    }
}
