using MaintenanceSchedule.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace MaintenanceSchedule
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
