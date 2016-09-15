using System.Web.Mvc;
using MaintenanceSchedule.Web.Controllers;

namespace MaintenanceSchedule.Web.Areas.Common.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        { }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Footer()
        {
            return PartialView("~/Views/Shared/_Footer.cshtml");
        }
    }
}
