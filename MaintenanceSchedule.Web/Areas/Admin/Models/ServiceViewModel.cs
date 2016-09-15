using MaintenanceSchedule.Core.Common.Configuration;
using ServiceEntity = MaintenanceSchedule.Entity.Datlichbaoduong.Service;

namespace MaintenanceSchedule.Web.Areas.Admin.Models
{
    public class ServiceViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DisplayImage { get; set; }
    }

    public static class ServiceViewModelExtension
    {
        public static ServiceViewModel ToViewModel(this ServiceEntity entity)
        {
            return new ServiceViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                DisplayImage = "<img alt='" + entity.Title + "' src='" + ConfigVariable.RelativeServiceThumbUrl + entity.Image + "' />"
            };
        }
    }
}
