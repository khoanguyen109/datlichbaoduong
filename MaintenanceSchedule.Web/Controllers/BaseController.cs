using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using FrameStore.Core.Mvc.Extensions;
using yellowx.Framework.Globalization;
using MaintenanceSchedule.Library.Utilities;
using MaintenanceSchedule.Core.Common.Configuration;

namespace MaintenanceSchedule.Web.Controllers
{
    public class BaseController : yellowx.Framework.Web.Mvc.Controller
    {
        public bool IsAjax
        {
            get
            {
                return Request.IsAjaxRequest();
            }
        }

        public int UserId
        {
            get
            {
                return User.Identity.IsAuthenticated ? Convert.ToInt32(User.Identity.GetUserId()) : 0;
            }
        }

        public string UserName
        {
            get
            {
                return User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
            }
        }

        protected string GetFormattedDate(DateTime? entryDate)
        {
            var date = entryDate.GetValueOrDefault();
            return "Ngày " + date.Day.ToString() + ", tháng " + date.Month.ToString() + ", năm " + date.Year.ToString();
        }

        protected void BuildSEO(SEOType type, string title = "", string id = "")
        {
            if (type == SEOType.Index)
            {
                ViewBag.FacebookUrl = ConfigVariable.MainDomainUrl;
                ViewBag.Title = @"Đặt lịch bảo dưỡng, bảo dưỡng định kỳ ô tô, chi phí bảo dưỡng định kỳ ôtô, 
                                bảng giá bảo dưỡng định kỳ xe hơi, dịch vụ bảo dưỡng ôtô! – datlichbaoduong.com";
            }
            else if (type == SEOType.ServiceDetail)
            {
                ViewBag.FacebookUrl = ConfigVariable.MainDomainUrl + "/dich-vu/" + title + "-" + id;
                ViewBag.Title = title + ". Dịch vụ đặt lịch bảo dưỡng, bảo dưỡng định kỳ ô tô – datlichbaoduong.com";
            }
        }

        protected IList<SelectListItem> GetDefaultDropdownList(List<DefaulDropdownValue> values)
        {
            return values.OrderBy(x => x.Text).ToSelectList(x => x.Text, x => x.Value, string.Empty);
        }

        protected JsonResult JsonResult(bool success, string message, object data = null, bool isGetMethod = true)
        {
            var jsonResultObject = new JsonResultObject { success = success, message = message, data = data };

            if (isGetMethod)
                return Json(jsonResultObject, JsonRequestBehavior.AllowGet);
            return Json(jsonResultObject);
        }

        protected JsonResult JsonErrorResult(string errorMessage, object data = null, bool isGetMethod = true)
        {
            return JsonResult(false, errorMessage, data, isGetMethod);
        }

        protected JsonResult JsonErrorResult(object data = null, bool isGetMethod = true)
        {
            return JsonResult(false, string.Empty, data);
        }

        protected JsonResult JsonSuccessResult(string successMessage, object data = null, bool isGetMethod = true)
        {
            return JsonResult(true, successMessage, data);
        }

        protected JsonResult JsonSuccessResult(object data = null, bool isGetMethod = true)
        {
            return JsonResult(true, string.Empty, data);
        }
    }

    public enum SEOType
    {
        None = 0,
        Index,
        ServiceDetail
    }

    public class DefaulDropdownValue
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    internal class JsonResultObject
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public string html { get; set; }
    }
}