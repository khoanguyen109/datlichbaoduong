using yellowx.Framework.Logging;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;

namespace yellowx.Framework
{
    /// <summary>
    /// Why does this Object exists? 
    /// We need an object to centralize all most common method such as : log, debug, helper method....
    /// </summary>
    public class Object
    {
        protected T[] Empty<T>()
        {
            return new T[0];
        }
        protected T Default<T>()
        {
            return default(T);
        }
        protected T[] Array<T>()
        {
            return new T[0];
        }
        protected IList<T> List<T>()
        {
            return new List<T>();
        }

        #region Logging methods
        /// <summary>
        /// Log message with a key. This key will point to the logger in configuration.
        /// </summary>
        /// <param name="key">The key registered in configuration</param>
        /// <param name="message">The message need to be logged.</param>
        protected void Log(string key, string message)
        {
            Log(key, message, null, null);
        }
        /// <summary>
        /// Log message with a key. This key will point to the logger in configuration.
        /// </summary>
        /// <param name="key">The key registered in configuration</param>
        /// <param name="message">The message need to be logged.</param>
        /// <param name="details">The key-value for detail information.</param>
        protected void Log(string key, string message, StringDictionary details)
        {
            Log(key, message, null, details);
        }
        /// <summary>
        /// Log message with a key. This key will point to the logger in configuration.
        /// </summary>
        /// <param name="key">The key registered in configuration</param>
        /// <param name="message">The message need to be logged.</param>
        /// <param name="exception">The exception need to be logged.</param>
        protected void Log(string key, string message, Exception exception)
        {
            Log(key, message, exception, null);
        }
        /// <summary>
        /// Log message with a key. This key will point to the logger in configuration.
        /// </summary>
        /// <param name="key">The key registered in configuration</param>
        /// <param name="message">The message need to be logged.</param>
        /// <param name="details">The key-value for detail information.</param>
        /// <param name="exception">The exception need to be logged.</param>
        protected void Log(string key, string message, Exception exception, StringDictionary details)
        {
            var logger = Configuration.Get<ILog>(key);
            if (logger != null)
                logger.Log(message, exception, details);
        }

        #endregion
    }
}
