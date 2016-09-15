using System.Collections.Generic;
using yellowx.Framework.Serializer;
using yellowx.Framework.Logging;
using System;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using yellowx.Framework.Globalization;
using yellowx.Framework.Dependency.Windsor;
using yellowx.Framework.Dependency;

namespace yellowx.Framework
{
    public class Configuration : Object
    {
        /// <summary>
        /// Registered object contains all types of object for application.
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<string, object>> registeredObjects = new Dictionary<Type, Dictionary<string, object>>();
        private static bool configurable = false; // prevent changing when app runs.
        private static ISerializer currentSerializer;
        private static ILog currentLogWriter;

        #region Properties
        public static ISerializer Serializer
        {
            get
            {
                return currentSerializer ?? new JsonNewtonSerializer();
            }
            set { currentSerializer = value; }
        }
        public static ILog LogWriter
        {
            get
            {
                return currentLogWriter ?? new LogWriter(new LogWriterConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\errors")));
            }
            set
            {
                currentLogWriter = value;
            }
        }
        public static string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["applicationName"] ?? "Unknown application name";
            }
        }

        public static ILocalizerProvider LocalizerProvider
        {
            get; set;
        }

        public static IObjectFactory ObjectFactory { get; set; }

        #endregion

        #region Public methods
        public static void Register<T>(string name, T target)
        {
            if (configurable) return;

            var keyType = typeof(T);
            if (!registeredObjects.ContainsKey(keyType))
                registeredObjects.Add(keyType, new Dictionary<string, object>());
            if (!registeredObjects[keyType].ContainsKey(name))
                registeredObjects[keyType].Add(name, target);
        }
        public static T Get<T>(string name) where T : class
        {
            var keyType = typeof(T);
            if (!registeredObjects.ContainsKey(keyType))
                return default(T);

            var o = registeredObjects[keyType][name];
            return o as T;
        }
        /// <summary>
        /// Shield the configuration from being changed.
        /// </summary>
        public static void Shield()
        {
            configurable = true;
        }
        #endregion

        #region Internal methods for Event source
        /// <summary>
        /// Write the message to the Output View for Debugging mode.
        /// </summary>
        /// <param name="message"></param>
        internal static void WriteDebugView(string message)
        {
            Debug.WriteLine(message);
        }
        internal static void WriteEventLogEntry(string message)
        {
            //WriteEventLogEntry(message, null);
        }
        internal static void WriteEventLogEntry(string message, System.Exception exception)
        {
            /*try
            {
                var sb = new System.Text.StringBuilder();
                if (exception != null)
                {
                    var ex = exception.InnerException ?? exception;
                    sb.AppendLine("Error: " + message);
                    sb.AppendLine("Exception: " + ex.Message);
                    sb.AppendLine("StackTrace: " + ex.StackTrace);
                    sb.AppendLine("Source: " + ex.Source);
                }
                var source = ApplicationName ?? "Unknown application";

                if (EventLog.SourceExists(source))
                    EventLog.WriteEntry(source, sb.ToString(), EventLogEntryType.Error);
            }
            catch
            {
                throw;
            }*/
        }
        #endregion
    }
}
