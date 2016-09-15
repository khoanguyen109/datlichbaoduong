using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using yellowx.Framework.Extensions;
using yellowx.Framework.Web.Mvc.Extensions;
using MaintenanceSchedule.Core.Common.Paging;
using MaintenanceSchedule.Web.Areas.Admin.Models;

namespace MaintenanceSchedule.Web.Binders
{
    public class SearchPageBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = new SearchPageForm();
            form.PageIndex = controllerContext.HttpContext.Request.GetInteger("current");
            form.PageSize = controllerContext.HttpContext.Request.GetInteger("rowCount");
            form.SearchTerm = controllerContext.HttpContext.Request.GetString("searchPhrase");
            form.Sorts = ParseSorts(controllerContext.HttpContext.Request);
            return form;
        }

        private IDictionary<string, SortDirection> ParseSorts(HttpRequestBase request)
        {
            var dictionary = new Dictionary<string, SortDirection>();
            var key = request.Form.AllKeys.FirstOrDefault(x => x.StartsWith("sort["));
            if (key != null)
            {
                var sortValue = request.Form[key] == "desc" ? SortDirection.Descending : SortDirection.Ascending;
                var sortKey = key.Replace(new[] { "sort[", "]" }, "");
                dictionary.Add(sortKey, sortValue);
            }
            return dictionary;
        }
    }
}
