using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace yellowx.Framework.Web.Serializer
{
    public class HtmlStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IHtmlString).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var source = value as IHtmlString;

            if (source == null)
                return;

            writer.WriteValue(source.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value == null ? MvcHtmlString.Empty : new MvcHtmlString(reader.Value.ToString());
        }
    }
}
