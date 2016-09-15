using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace yellowx.Framework.Exceptions
{
    [Serializable]
    public class DependentComponentException<T> : AppException
    {
        public DependentComponentException()
            : base(DefaultMessage())
        { }

        public DependentComponentException(bool logged)
            : base(DefaultMessage(), logged)
        { }

        public DependentComponentException(string componentName)
            : base(DefaultMessage(componentName))
        { }

        public DependentComponentException(bool logged, string componentName)
            : base(DefaultMessage(componentName), logged)
        { }


        public DependentComponentException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }

        private static string DefaultMessage()
        {
            // this string can be replaced by locale sensitive 
            // resource entries or something flash liks this later on
            return string.Format("A dependent component of type '{0}' is null or otherwise invalid; execution is aborted",
                typeof(T).Name);
        }

        private static string DefaultMessage(string componentName)
        {
            // this string can be replaced by locale sensitive 
            // resource entries or something flash liks this later on
            return string.Format("Dependent component of type '{0}' and name '{1}' is invalid; execution is aborted",
                typeof(T).Name,
                componentName);
        }
    }
}
