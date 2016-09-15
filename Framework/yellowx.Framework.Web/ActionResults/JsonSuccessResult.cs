using System;
using System.Web.Mvc;
using yellowx.Framework.Web.Notifications;

namespace yellowx.Framework.Web.ActionResults
{
    public class JsonSuccessResult : JsonNetResult
    {
        public JsonSuccessResult(object data = null, bool allowGet = false, string message = null, Enum status = null, UiNotificationType? notificationType = null, string redirect = null, bool reload = false)
        {
            Data = new JsonResultObject
            {
                success = true,
                data = data ?? string.Empty,
                redirect = redirect ?? string.Empty,
                message = message ?? string.Empty,
                status = status == null ? "" : status.ToString(),
                notificationtype = notificationType,
                reload = reload,
            };

            if (allowGet)
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}
