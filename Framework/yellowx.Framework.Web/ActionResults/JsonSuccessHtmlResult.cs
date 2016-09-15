using System.Collections.Generic;
using System.Web.Mvc;
using yellowx.Framework.Web.Notifications;

namespace yellowx.Framework.Web.ActionResults
{
    public class JsonSuccessHtmlResult : JsonNetResult
    {
        public JsonSuccessHtmlResult(Dictionary<string, string> htmlsById, object data = null, string message = null, UiNotificationType? notificationType = null, string status = null)
        {
            Data = new JsonResultObject
            {
                success = true,
                htmlsById = htmlsById,
                data = data,
                message = message,
                notificationtype = notificationType,
                status = status,
            };
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }

        public JsonSuccessHtmlResult(string html, object data = null, Dictionary<string, string> htmlsById = null, string message = null, UiNotificationType? notificationType = null, string status = null)
        {
            Data = new JsonResultObject
            {
                success = true,
                html = html,
                data = data,
                htmlsById = htmlsById,
                message = message,
                notificationtype = notificationType,
                status = status,
            };
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}