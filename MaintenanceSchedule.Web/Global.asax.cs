using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using MaintenanceSchedule.Web.Binders;
using MaintenanceSchedule.Data.Vienauto;
using MaintenanceSchedule.Library.NHibernate;
using MaintenanceSchedule.Web.Areas.Admin.Models;
using MaintenanceSchedule.Data.DatlichbaoduongMaps;
using System.Web.Helpers;
using System.Security.Claims;

namespace MaintenanceSchedule
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.Add(typeof(SearchPageForm), new SearchPageBinder());
            InitializeNHibernate();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            SessionFactory.Instance.CloseSessions();
        }

        private static object lockObject = new object();

        private static bool wasInitialized = false;
        private void InitializeNHibernate()
        {
            if (!wasInitialized)
            {
                lock (lockObject)
                {
                    if (!wasInitialized)
                    {
                        wasInitialized = true;
                        //var mapAssemblies = new[]
                        //{
                        //     HostingEnvironment.MapPath("~/bin/MaintenanceSchedule.Data.dll")
                        //};
                        SessionFactory.Instance.Initialize("nhibernate.current_factory_key", "DatlichbaoduongConnectionString", typeof(ServiceMap).Assembly);
                        SessionFactory.Instance.Initialize("nhibernate.vienauto_factory_key", "VienautoConnectionString", typeof(ManufacturerMap).Assembly);
                    }
                }
            }
        }
    }
}
