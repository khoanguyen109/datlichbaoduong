using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace yellowx.Framework.Serializer
{
    public class JsonNewtonSerializer : Serializer
    {
        public override T Deserialize<T>(StreamReader stream)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff" });
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            using (var jsonReader = new JsonTextReader(stream))
            {
                var ret = serializer.Deserialize<T>(new JsonTextReader(stream));
                return ret;
            }
        }

        public override object Deserialize(StreamReader stream)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff" });
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            using (var jsonReader = new JsonTextReader(stream))
            {
                var ret = serializer.Deserialize(new JsonTextReader(stream));
                return ret;
            }
        }
        protected override void Serialize<T>(StreamWriter streamWriter, T graph)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff" });
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonWriter, graph);
            }
        }

    }
}
