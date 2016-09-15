using System.IO;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace yellowx.Framework.Serializer
{
    /// <summary>
    /// Encapsulates how to serialize an object using BinaryFormatter.
    /// </summary>
    public class BinarySerializer : Serializer
    {
        public override T Deserialize<T>(StreamReader streamReader)
        {
            var formatter = new BinaryFormatter();
            var item = (T)formatter.Deserialize(streamReader.BaseStream);
            return item;
        }

        public override object Deserialize(StreamReader streamReader)
        {
            var formatter = new BinaryFormatter();
            var item = formatter.Deserialize(streamReader.BaseStream);
            return item;
        }

        protected override void Serialize<T>(StreamWriter streamWriter, T graph)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(streamWriter.BaseStream, graph);
        }
    }
}
