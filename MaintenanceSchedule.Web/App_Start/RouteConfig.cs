using System.Web.Mvc;
using System.Web.Routing;

namespace MaintenanceSchedule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",                                              
                url: "{controller}/{action}/{id}",                           
                defaults: new
                {
                    controller = "Service",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "MaintenanceSchedule.Web.Areas.Service.Controllers" }
            ).DataTokens.Add("area", "Service");
        }
    }
}

