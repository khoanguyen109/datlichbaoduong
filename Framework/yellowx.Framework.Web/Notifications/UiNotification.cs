using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using yellowx.Framework.Web.Serializer;

namespace yellowx.Framework.Web.Notifications
{
    public class UiNotification
    {
        [JsonConverter(typeof(HtmlStringConverter))]
        public MvcHtmlString Message { get; set; }
        public UiNotificationType Type { get; set; }

        public UiNotification()
        { }

        public UiNotification(UiNotificationType type, MvcHtmlString message)
        {
            Message = message;
            Type = type;
        }

        public UiNotification(UiNotificationType type, string message)
        {
            Message = MvcHtmlString.Create(HttpUtility.HtmlEncode(message));
            Type = type;
        }

        public string DecodedMessage
        {
            get
            {
                return HttpUtility.HtmlDecode(Message.ToString());
            }
        }

        public static UiNotification Error(MvcHtmlString message)
        {
            return new UiNotification(UiNotificationType.Error, message);
        }

        public static UiNotification Error(string unencodedMessage)
        {
            return Error(new MvcHtmlString(HttpUtility.HtmlEncode(unencodedMessage)));
        }

        public static UiNotification Confirmation(MvcHtmlString message)
        {
            return new UiNotification(UiNotificationType.Confirmation, message);
        }

        public static UiNotification Confirmation(string unencodedMessage)
        {
            return Confirmation(new MvcHtmlString(HttpUtility.HtmlEncode(unencodedMessage)));
        }

        public static UiNotification Info(MvcHtmlString message)
        {
            return new UiNotification(UiNotificationType.Info, message);
        }

        public static UiNotification Info(string unencodedMessage)
        {
            return Info(new MvcHtmlString(HttpUtility.HtmlEncode(unencodedMessage)));
        }
    }

    public enum UiNotificationType
    {
        Error,
        Confirmation,
        Info,
        Preview,
    }
}
