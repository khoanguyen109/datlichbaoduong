using System;
using System.Web.Mvc;
using yellowx.Framework.UnitWork;
using yellowx.Framework.Extensions;
using MaintenanceSchedule.Web.Controllers;
using MaintenanceSchedule.Core.Common.Paging;
using MaintenanceSchedule.Web.Areas.Admin.Models;

namespace MaintenanceSchedule.Web.Areas.Admin.Controllers
{
    public class AdminController : BaseController
    {
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Page<QueryRequest, QueryResponse, ViewModel, DomainModel>(SearchPageForm searchPageForm,
            IQuery<QueryRequest, QueryResponse> query,
            Func<DomainModel, ViewModel> Transformer)
            where QueryRequest : PagingRequest, new()
            where QueryResponse : PagingResponse<DomainModel>
        {
            var response = query.Invoke(new QueryRequest()
            {
                PageIndex = searchPageForm.PageIndex - 1,
                PageSize = searchPageForm.PageSize,
                SearchTerm = searchPageForm.SearchTerm
            });

            var models = response.Items.ForEach(i => Transformer(i));
            return Json(new
            {
                current = searchPageForm.PageIndex,
                rowCount = searchPageForm.PageSize,
                total = response.TotalCount,
                rows = models
            }, JsonRequestBehavior.AllowGet);
        }

        protected void ShowDescribesPage(DescribeType describeType, string label)
        {
            if (describeType == DescribeType.Create)
            {
                ViewBag.Link = "/Admin/" + label;
                ViewBag.Text = "Xem danh sách " + label.GetDescription();
                ViewBag.PageHeader = "Tạo " + label.GetDescription() + " mới";
            }
            else if (describeType == DescribeType.Edit)
            {
                ViewBag.Link = "/Admin/" + label;
                ViewBag.Text = "Xem danh sách " + label.GetDescription();
                ViewBag.PageHeader = "Cập nhật thông tin " + label.GetDescription();
            }
            else if (describeType == DescribeType.List)
            {
                ViewBag.PageHeader = "Danh sách " + label.GetDescription();
            }
        }
    }

    public enum DescribeType
    {
        None = 0,
        Create,
        Edit,
        List
    }

    public static class Describe
    {
        public const string ServiceLabel = "Service";

        public static string GetDescription(this string label)
        {
            var rawLabel = string.Empty;
            switch (label)
            {
                case "Service":
                    rawLabel = "dịch vụ";
                    break;
                default:
                    rawLabel = "dịch vụ";
                    break;
            }
            return rawLabel;
        }
    }
}