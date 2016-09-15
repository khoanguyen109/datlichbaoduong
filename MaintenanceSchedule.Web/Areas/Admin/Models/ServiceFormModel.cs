using MaintenanceSchedule.Core.Common.Configuration;
using System.Collections.Generic;

namespace MaintenanceSchedule.Web.Areas.Admin.Models
{
    public class ServiceFormModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public string DisplayImage
        {
            get
            {
                return !string.IsNullOrEmpty(Image) ? ConfigVariable.RelativeServiceMediumUrl + Image : string.Empty;
            }
        }
        public string Message { get; set; }
    }
}
