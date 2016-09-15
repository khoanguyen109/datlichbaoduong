using System.Text;
using System.IO;
using System.Collections.Specialized;
using yellowx.Framework.IO;
using System;

namespace yellowx.Framework.Serializer
{
    public interface ISerializer
    {
        T Deserialize<T>(string fullFilePath) where T : class;
        T Deserialize<T>(StreamReader stream) where T : class;
        T Deserialize<T>(byte[] bytes) where T : class;

        object Deserialize(byte[] bytes);
        object Deserialize(StreamReader stream);

        void Serialize<T>(string fullFilePath, T graph);
        string Serialize<T>(T graph);
    }

    public abstract class Serializer : ISerializer
    {
        private readonly Streamer streamer = new Streamer();

        #region ISerializer Members

        public T Deserialize<T>(string fullFilePath) where T : class
        {
            var result = default(T);
            try
            {
                using (var acquirer = streamer.AcquireReader(fullFilePath))
                {
                    if (acquirer != null)
                        result = Deserialize<T>(acquirer);
                }
            }
            catch (System.Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Method", "Deserialize");
                details.Add("Item type", typeof(T).FullName);
                details.Add("File Path", "fullFilePath");
                Configuration.LogWriter.Log("Unable to Deserialize.", ex, details);
            }
            return result;
        }

        public T Deserialize<T>(byte[] bytes) where T : class
        {
            var result = default(T);
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    result = Deserialize<T>(new StreamReader(ms));
                }
            }
            catch (System.Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Method", "Deserialize");
                details.Add("Item type", typeof(T).FullName);
                Configuration.LogWriter.Log("Unable to Deserialize (Bytes)", ex, details);
            }
            return result;
        }

        public object Deserialize(byte[] bytes)
        {
            var o = default(object); ;
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    o = Deserialize(new StreamReader(ms));
                }
            }
            catch (System.Exception ex)
            {
                var details = new StringDictionary();
                details.Add("Method", "Deserialize");
                Configuration.LogWriter.Log("Unable to Deserialize (Bytes)", ex, details);
            }
            return o;

        }

        /// <summary>
        /// De-serialize a stream content into an object.
        /// </summary>
        public abstract T Deserialize<T>(StreamReader streamReader) where T : class;

        /// <summary>
        /// De-serialize a stream content into an object.
        /// </summary>
        public abstract object Deserialize(StreamReader streamReader);

        /// <summary>
        /// Serialize an object T to a file.
        /// </summary>
        public void Serialize<T>(string fullFilePath, T graph)
        {
            using (var writer = streamer.AcquireWriter(fullFilePath))
            {
                if (writer == null) return;
                try
                {
                    Serialize(writer, graph);
                }
                catch (System.Exception ex)
                {
                    var details = new StringDictionary();
                    details.Add("Method", "Serialize");
                    details.Add("Item type", typeof(T).FullName);
                    details.Add("File path", fullFilePath);
                    Configuration.LogWriter.Log("Unable to Serialize.", ex, details);
                }
            }
        }

        /// <summary>
        /// Serialize an object T to a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="graph"></param>
        /// <returns></returns>
        public string Serialize<T>(T graph)
        {
            var result = string.Empty;
            if (graph == null) return result;

            using (var ms = new MemoryStream())
            {
                try
                {
                    Serialize(new StreamWriter(ms), graph);
                    result = Encoding.Default.GetString(ms.GetBuffer());
                    return result;
                }
                catch (Exception ex)
                {
                    var details = new StringDictionary();
                    details.Add("Method", "Serialize");
                    details.Add("Item type", typeof(T).FullName);
                    Configuration.LogWriter.Log("Unable to Serialize.", ex, details);
                }
            }
            return result;
        }

        #endregion

        #region Protected virtual methods
        protected virtual void Serialize<T>(StreamWriter streamWriter, T graph) { }
        #endregion
    }
}
