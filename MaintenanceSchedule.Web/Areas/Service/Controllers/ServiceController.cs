using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using yellowx.Framework.Data.Paging;
using FrameStore.Core.Mvc.Extensions;
using MaintenanceSchedule.Web.Controllers;
using MaintenanceSchedule.Library.Utilities;
using MaintenanceSchedule.Core.Queries.Vienauto;
using MaintenanceSchedule.Web.Areas.Common.Models;
using MaintenanceSchedule.Web.Areas.Service.Models;
using MaintenanceSchedule.Core.Common.Configuration;
using MaintenanceSchedule.Core.Queries.Datlichbaoduong;

namespace MaintenanceSchedule.Web.Areas.Service.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceQuery _serviceQuery;
        private readonly IGetYearListQuery _getYear;
        private readonly IGetModelListQuery _getModel;
        private readonly IGetAllManufacturerListQuery _allManufactureQuery;
        
        public ServiceController(IServiceQuery serviceQuery, IGetAllManufacturerListQuery allManufactureQuery,
                                 IGetModelListQuery getModel, IGetYearListQuery getYear)
        {
            _serviceQuery = serviceQuery;
            _allManufactureQuery = allManufactureQuery;
            _getModel = getModel;
            _getYear = getYear;
        }

        public ActionResult View(int id, string title)
        {
            var service = _serviceQuery.GetService(id);
            ViewBag.ServiceId = id.ToString();
            BuildSEO(SEOType.ServiceDetail, service.Title, id.ToString());
            var viewModel = LoadViewModel();
            return View(viewModel);
        }

        public ActionResult Detail(int id)
        {
            var service = _serviceQuery.GetService(id);
            //BuildSEO(SEOType.ServiceDetail, service.Title, id.ToString());

            var nextService = _serviceQuery.GetNextService(id);
            var lastService = _serviceQuery.GetLastService(id);

            if (service != null)
            {
                return PartialView("~/Areas/Service/Views/Service/_Detail.cshtml", new ServiceViewModel
                {
                    Id = service.Id,
                    Tags = service.Tags,
                    Title = service.Title,
                    Image = service.Image,
                    Content = service.Content,
                    EntryDate = service.EntryDate,
                    Description = service.Description,
                    ListLatestServices = _serviceQuery.GetLatestService(),
                    NextService = nextService != null ? nextService.Id : 0,
                    PreviousService = lastService != null ? lastService.Id : 0
                });
            }
            return PartialView("~/Areas/Service/Views/Service/_Detail.cshtml", new ServiceViewModel());
        }

        public ActionResult Index()
        {
            BuildSEO(SEOType.Index);
            var viewModel = (HomeViewModel)TempData["HomeViewModel"];
            viewModel = LoadViewModel(viewModel);
            return View(viewModel);
        }

        public ActionResult List()
        {
            return PartialView("~/Areas/Service/Views/Service/_Index.cshtml");
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel viewModel)
        {
            var viewModelReturn = new HomeViewModel();
            viewModelReturn.Message = string.Empty;

            try
            {
                var mailContentDictionary = new Dictionary<string, string>
                {
                    { "MANUFACTURER", viewModel.ManufacturerName },
                    { "MODEL", viewModel.ModelName },
                    { "YEAR", viewModel.YearName },
                    { "CONTENT", viewModel.Content },
                    { "CUSTOMERNAME", viewModel.CustomerName },
                    { "CUSTOMERPHONE", viewModel.CustomerPhone }
                };
                Mail.Send(ConfigVariable.MailFrom, ConfigVariable.MailTo, new string[] { }, mailContentDictionary, "Đặt lịch bảo dưỡng");
                viewModelReturn.Message = "Chúc mừng bạn đã đặt lịch bảo dưỡng thành công, chúng tôi sẽ ưu tiên xe của bạn!";
            }
            catch (Exception ex)
            {
                viewModelReturn.Message = ex.ToString();
            }

            LoadViewModel(viewModelReturn);
            TempData["HomeViewModel"] = viewModelReturn;

            return RedirectToRoute("Default");
        }

        public ActionResult GetServices(int index, int size)
        {
            PagingResult<MaintenanceSchedule.Entity.Datlichbaoduong.Service> result;

            var paging = new Paging { Index = index, Size = size };
            result = _serviceQuery.GetServices(paging);

            var totalPages = Convert.ToInt32(Math.Ceiling(result.TotalCount * 1.0 / size));

            var pages = new List<Pages>();
            for (var i = 1; i <= totalPages; i++)
                pages.Add(new Pages { Page = i });

            return JsonSuccessResult(new
            {
                pages = pages,
                currentIndex = index,
                items = result.Items.Select(x =>
                new ServiceViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Image = x.Image,
                    Content = x.Content,
                    Description = !string.IsNullOrEmpty(x.Description) ? Characters.Chop(x.Description, 100) : string.Empty
                }).ToList(),
            });
        }

        public ActionResult LoadModelByManufacturer(string manufacturerId)
        {
            var response = _getModel.Invoke(new GetModelListQueryRequest { ManufacturerId = int.Parse(manufacturerId) }).Items.OrderBy(m => m.Name);
            return JsonSuccessResult(response);
        }

        public ActionResult LoadYearByModel(string modelId)
        {
            var response = _getYear.Invoke(new GetYearListQueryRequest { ModelId = int.Parse(modelId) }).Items.OrderBy(y => y.Name);
            return JsonSuccessResult(response);
        }

        private HomeViewModel LoadViewModel(HomeViewModel viewModel = null)
        {
            if (viewModel == null)
                viewModel = new HomeViewModel();

            var manufacturers = _allManufactureQuery.Invoke(new GetAllManufacturerListQueryRequest()).Items;

            var listViewManufacturers = manufacturers.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();
            listViewManufacturers.Insert(0, new SelectListItem { Text = "Chọn hãng xe", Value = "0" });

            var logoStrings = new string[] { "Toyota", "Hyundai", "Mazda", "Nissan", "Kia", "Acura", "Lexus", "BMW",
                                             "Fiat", "Ford", "Chrysler", "Chevrolet", "Honda", "Cadillac", "Volkswagen" };

            viewModel.ManufacturerId = viewModel.ManufacturerId != 0 ? viewModel.ManufacturerId : 0;
            viewModel.Manufacturers = listViewManufacturers;
            viewModel.Logos = manufacturers.Where(x => logoStrings.Contains(x.Name))
                                           .OrderBy(x => x.Name)
                                           .Select(x => new LogoViewModel { Alt = x.Name, Path = x.Logo }).ToList();
            return viewModel;
        }
    }
}