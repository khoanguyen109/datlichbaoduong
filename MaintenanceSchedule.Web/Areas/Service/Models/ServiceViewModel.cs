using System;
using System.Collections.Generic;
using MaintenanceSchedule.Library.Utilities;
using MaintenanceSchedule.Core.Common.Configuration;
using ServiceEntity = MaintenanceSchedule.Entity.Datlichbaoduong.Service;

namespace MaintenanceSchedule.Web.Areas.Service.Models
{
    public class ServiceViewModel
    {
        public ServiceViewModel()
        {
            ListLatestServices = new List<ServiceEntity>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleFriendlyName
        {
            get
            {
                return Characters.ConvertToUnSign3(Title);
            }
        }
        public string Image { get; set; }
        public string DisplayImage
        {
            get
            {
                return !string.IsNullOrEmpty(Image) ? ConfigVariable.RelativeServiceMediumUrl + Image : string.Empty;
            }
        }
        public string Content { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryDateFormatted
        {
            get
            {
                var entryDate = EntryDate.HasValue ? (DateTime)EntryDate : DateTime.Now;
                var day = entryDate.Day.ToString();
                var month = entryDate.Month.ToString();
                var year = entryDate.Year.ToString();
                return day + " tháng " + month + ", năm " + year;
            }
        }
        public int NextService { get; set; }
        public int PreviousService { get; set; }
        public IList<ServiceEntity> ListLatestServices { get; set; }
    }

    public class Pages
    {
        public int Page { get; set; }
    }
}
