using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Vendare.Error;


namespace Vendare.Xml
{
    /// <summary>
    /// A class that provides schema validation for xml. This is done using a static class
    /// so that on each validation the schema need not be read and compiled.
    /// </summary>
    public class XmlValidator
    {
        private static Dictionary<XmlReader, NameValueCollection> states = new Dictionary<XmlReader, NameValueCollection>();
        private static Dictionary<string, XmlReaderSettings> settingsCollection = new Dictionary<string, XmlReaderSettings>();
        // locking needed for static settings and static namevalue collections
        private static LockObject lockObject = new LockObject();
        private static int counter = 0;

        /// <summary>
        /// Constructor. 
        /// </summary>
        static XmlValidator() { }

        /// <summary>
        /// Initialize a XmlReaderSettings instance for a given schema
        /// </summary>
        /// <param name="schemauri"></param>
        /// <returns></returns>
        private static XmlReaderSettings InitSetting(string schemauri)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ValidationEventHandler += new ValidationEventHandler(SchemaValidationHandler);

            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(null, schemauri);
            settings.ValidationFlags =
            XmlSchemaValidationFlags.ReportValidationWarnings |
            XmlSchemaValidationFlags.ProcessIdentityConstraints |
            XmlSchemaValidationFlags.ProcessInlineSchema |
            XmlSchemaValidationFlags.ProcessSchemaLocation;

            settingsCollection.Add(schemauri, settings);

            return settings;
        }

        /// <summary>
        /// Validate an Xml String. This overload will report line numbers
        /// </summary>
        /// <param name="schemauri"></param>
        /// <param name="xmlstring">a string containing hopefully valid xml </param>
        /// <param name="reportWarnings">Whether warnings will be included in returned NameValueCollection (errors are always included).</param>
        /// <returns>NameValueCollection with at least one entry if errors (or warnings if reportWarnings is true) or null</returns>
        public static NameValueCollection Validate(string schemauri, string xmlstring, bool reportWarnings)
        {
            XmlReader validatingReader = null;
            XmlReaderSettings settings = null;
            NameValueCollection nvc = null;

            try
            {
                // add settings for the schema is not already present
                lock (lockObject)
                {
                    if (!settingsCollection.ContainsKey(schemauri))
                        settings = InitSetting(schemauri);
                    else
                        settings = settingsCollection[schemauri];

                    // nvc is per-validation
                    nvc = new NameValueCollection();
                    StringReader r = new StringReader(xmlstring);
                    validatingReader = XmlReader.Create(r, settings);

                    // SchemaValidationHandler uses XmlReader for state and, through it,
                    // obtains nvc to which errors and warnings are accumulated
                    states.Add(validatingReader, nvc);
                }

                while (validatingReader.Read()) { /* just loop through document */ }
            }
            catch (Exception ex)
            {
                new LoggableException(ex, null);
            }
            finally
            {
                // always remove nvc when validation over
                if (validatingReader != null)
                {
                    nvc = states[validatingReader];
                    lock (lockObject)
                    {
                        states.Remove(validatingReader);
                    }
                }
                else
                {
                    new LoggableException("Problem in XmlValidator. validatingReader is null.", null);
                }
            }

            // if warnings not desired then remove them
            if (!reportWarnings)
            {
                if (nvc != null)
                {
                    List<string> list = new List<string>();
                    foreach (string key in nvc.Keys)
                        list.Add(key);
                    foreach (string key in list)
                        if (key.StartsWith("WARNING"))
                            nvc.Remove(key);
                    if (nvc.Count == 0)
                        nvc = null;
                }
            }

            return nvc;
        }

        /// <summary>
        /// Validation Event Callback
        /// </summary>
        /// <param name="sender">object emitting event</param>
        /// <param name="e">ValidationEventArgs object</param>
        private static void SchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            // increment counter that makes each emitted message unique
            // if counter starts to wrap then reset it
            int oldval = -1;
            int comparand = 0;
            do
            {
                comparand = counter;
                int newcounter = comparand + 1;
                if (newcounter == int.MaxValue - 1000)
                    newcounter = 0;
                oldval = Interlocked.CompareExchange(ref counter, newcounter, comparand);
            }
            while (oldval != comparand);

            NameValueCollection nvc = null;
            lock (lockObject)
            {
                nvc = states[(XmlReader)sender];
            }

            // if this validation has been provided a place to accumulate errors
            if (nvc != null)
            {
                string type = "ERROR";
                if (e.Severity == XmlSeverityType.Warning)
                    type = "WARNING";
                nvc.Add(type + counter.ToString(), type + ": Line " + e.Exception.LineNumber.ToString() + ", Column: " + e.Exception.LinePosition.ToString() + ". " + e.Message);
            }
        }

        /// <summary>
        /// object used to serialize static collections for settings and state 
        /// </summary>
        private class LockObject
        {
            public LockObject() { }
        }
    }
}
