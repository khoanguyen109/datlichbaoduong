using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace yellowx.Framework.Exceptions
{
    public abstract class AppException : System.Exception
    {
        /// <summary>
        /// Flag indicating whether the exception has been logged
        /// </summary>
        public bool Logged { get; set; }

        protected AppException()
        { }

        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        protected AppException(string message)
            : this(message, false)
        { }

        protected AppException(string message, bool logged)
            : base (message)
        {
            Logged = logged;
        }

        protected AppException(string message, bool logged, System.Exception innerException)
            : base (message, innerException)
        {
            Logged = logged;
        }
    }
}
