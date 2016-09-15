using System;
using System.Collections.Specialized;

namespace Vendare.Utils
{
	/// <summary>
	/// Summary description for LogInfo.
	/// </summary>
	public struct LogInfo
	{
		private LogLevel level;
		private String logName;
		private String owner;
		private String msg;
		private NameValueCollection detail;
		private bool writeToEventLog;

		public LogInfo(LogLevel level, String logName, String owner, String msg, NameValueCollection detail, bool writeToEventLog)
		{
			this.level = level;
			this.logName = logName;
			this.owner = owner;
			this.msg = msg;
			this.detail = detail;
			this.writeToEventLog = writeToEventLog;
		}

		public LogLevel Level 
		{
			get { return level; }
		}

		public String LogName 
		{
			get { return logName; }
		}

		public String Owner 
		{
			get { return owner; }
		}

		public String Message 
		{
			get { return msg; }
		}

		public NameValueCollection Detail 
		{
			get { return detail; }
		}
		public bool WriteToEventLog
		{
			get {return writeToEventLog;}
		}
	}
}
