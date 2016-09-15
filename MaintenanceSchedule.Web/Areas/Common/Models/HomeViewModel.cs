using System.Web.Mvc;
using System.Collections.Generic;
using MaintenanceSchedule.Core.Common.Configuration;

namespace MaintenanceSchedule.Web.Areas.Common.Models
{
    public class HomeViewModel
    {
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string ModelName { get; set; }
        public string YearName { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Message { get; set; }
        public IList<SelectListItem> Manufacturers { get; set; }
        public IList<LogoViewModel> Logos { get; set; }
    }

    public class LogoViewModel
    {
        public string Alt { get; set; }
        public string Path { get; set; }
        public string PathUrl
        {
            get
            {
                return ConfigVariable.PrimaryDomainUrl + "/upload/" + Path;
            }
        }
    }
}
