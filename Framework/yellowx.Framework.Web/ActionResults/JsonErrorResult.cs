using System.Web.Mvc;
using yellowx.Framework.Web.Notifications;

namespace yellowx.Framework.Web.ActionResults
{
    public class JsonErrorResult : JsonNetResult
    {
        public JsonErrorResult(string errorMessage = null, string redirect = null, bool allowGet = false, object data = null)
        {
            Data = new JsonResultObject
            {
                success = false,
                message = errorMessage ?? string.Empty,
                redirect = redirect ?? string.Empty,
                data = data,
                notificationtype = UiNotificationType.Error,
            };

            if(allowGet)
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}
