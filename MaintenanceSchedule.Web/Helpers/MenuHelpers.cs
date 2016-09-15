using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MaintenanceSchedule.Web.Helpers
{
    public static class MenuHelpers
    {
        public static MvcHtmlString Menu(this HtmlHelper helper)
        {
            return helper.Partial("~/Areas/Admin/Views/Shared/_Menu.cshtml");
        }
    }
}
