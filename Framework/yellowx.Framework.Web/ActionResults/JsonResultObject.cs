using System.Collections.Generic;
using yellowx.Framework.Web.Notifications;

namespace yellowx.Framework.Web.ActionResults
{
    public class JsonResultObject
    {
        public JsonResultObject()
        {
            success = false;
            message = "";
            redirect = "";
            data = "";
            html = "";
            notificationtype = null;
            reload = false;
        }

        public bool success { get; set; }
        public string message { get; set; }
        public string redirect { get; set; }
        public bool reload { get; set; }
        public object data { get; set; }
        public string html { get; set; }
        public string status { get; set; }
        public UiNotificationType? notificationtype { get; set; }
        public Dictionary<string, string> htmlsById { get; set; }
    }
}