using System;
using System.Collections.Specialized;
using System.Text;

namespace yellowx.Framework.Logging
{
    public class LogItem
    {
        private readonly string _message;
        private readonly string _machineName;
        private readonly DateTime _entryDate;
        private readonly System.Exception _exception;
        private readonly StringDictionary _details;

        public LogItem(string message, System.Exception exception, StringDictionary details)
        {
            _message = message;
            _exception = exception;
            _details = details;
            _entryDate = DateTime.Now;
            _machineName = Environment.MachineName;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_message);
            return sb.ToString();
        }
    }
}
