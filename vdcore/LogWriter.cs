using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Vendare.Error;

namespace Vendare.Utils
{
	/// <summary>
	/// Defines logging levels for log messages
	/// LogLevels are specified in two places
	/// 1. In the configuration file
	/// 2. In the LogWriter.Write method
	/// If a message is sent to .Write at a higher level than the
	/// LogLevel specified in the config(None being the lowest),
	/// then the message is ignored and not logged.
	/// </summary>
	public enum LogLevel 
	{
		None,
		Info,
		Debug
	}

	/// <summary>
	/// This class is used to log messages and accompanying detail to a log file.
	/// There can be many log files. Each defined in the config file's application
	/// settings. A log file is defined in the config with 2 application settings:
	/// 1. key="fileName:[logName]" - this defines the absolute path of the log file
	/// 2. key="logLevel:[logName]" - this defines the logLevel which incoming messages should
	///		be evaluated against to determine if they should be logged.
	///	In addition to logs created with these settings there ids a default log used
	///	when no log name is passed to LogWriter.Write.
	///	
	///	The LogWriter only exposes a single static Write method with several overloaded
	///	implementations. The LogWriter class maintains a private static hash table of all
	///	logs currently used. The first time that a log receives a message, the log is created
	///	and stored in the hash table. Thus a single instance of the log can service 
	///	multiple requests.
	/// </summary>
	public class LogWriter
	{
		private static Hashtable logs = new Hashtable();
		//this timer checks the logs once a minute and closes logs that have had at least a minute of inactivity
		private static Timer activityTimer = new Timer(new TimerCallback(CheckLogActivity),null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
       	private StreamWriter stream = null;
		private StringBuilder buff = null;
		private String name;
		private String fileName;
		private LogLevel logLevel = LogLevel.None;
		private FileSystemWatcher watcher;
		private int archiveIndex = 1;
		private DateTime lastWrite;

        private Timer refreshTimer = null;

		private static int archiveLimit = 5;
		private static int fileSize = 1048576;

		static LogWriter() 
		{
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(LogWriter.ShutDown);
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(LogWriter.ShutDown);

			try 
			{
				archiveLimit = Int32.Parse(ConfigurationManager.AppSettings.Get("archiveLimit"));
			}
			catch {}
			try 
			{
				fileSize = Int32.Parse(ConfigurationManager.AppSettings.Get("logFileSize"));
			}
			catch {}

		}

		/// <summary>
		/// Creates an instance of a LogWriter
		/// </summary>
		/// <param name="logName">The name associated with this log</param>
		private LogWriter(String logName) 
		{
			this.name = logName;

			//get the config settings
			fileName = ConfigurationManager.AppSettings.Get("fileName:" + logName);
			String strLogLevel = ConfigurationManager.AppSettings.Get("logLevel:" + logName);

			//throw an error if the settings are missing
			if(fileName==null || strLogLevel==null) 
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("LogName", logName);
				new LoggableException("Cannot find config settings for this log file.", detail);
			}
			else 
			{
				String appName = ConfigurationManager.AppSettings.Get("appName");
				fileName = fileName.Replace("%appName%",appName);

				//set the logging level
				logLevel = (LogLevel)LogLevel.Parse(typeof(LogLevel), strLogLevel, true);

				AttemptStream();
			}

            refreshTimer = new Timer(new TimerCallback(OnTimeout), null, new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 5));

		}

		private void Write(LogLevel level, String owner, String msg, NameValueCollection detail) 
		{
			try 
			{
				//don't log if the loglevel in the config is less than the specified loglevel
				if((int)logLevel < (int)level)
					return;

				StringBuilder output = new StringBuilder(100);
				System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

				//write time and owner
				output.Append("[");
                output.Append(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.ffffff"));
				output.Append(" - PID: ");
				output.Append(currentProcess.Id.ToString());
				output.Append("]");
				if(owner != null) 
				{
					output.Append(" - OWNER:");
					output.Append(owner.ToUpper());
				}
				output.Append("\r\n");

				//write the message
				output.Append(msg);
				output.Append("\r\n");

				//write details
				if(detail != null) 
				{
					String[] keys = detail.AllKeys;
					for(int i=0; i < keys.Length; i++) 
					{
						output.Append("\t");
						output.Append(keys[i].Trim().ToUpper());
						output.Append("=");
						output.Append(detail.Get(keys[i]));
						output.Append("\r\n");
					}
				}

				output.Append("************************************************");

				lock(this) 
				{
					if(stream == null)
						AttemptStream();

					if(stream != null) 
					{
						stream.WriteLine(output.ToString());
						stream.Flush();
					}
					else
						buff.Append(output.ToString());

					lastWrite = DateTime.Now;
				}
			}
			catch(LoggableException ){}
			catch(Exception e) 
			{
				new LoggableException(e, null);
			}

		}

		private void AttemptStream() 
		{
			//open the file, or create it
			try 
			{
				stream = File.AppendText(fileName);
			}
			catch(FileNotFoundException) 
			{
				stream = File.CreateText(fileName);
			}
			catch(DirectoryNotFoundException) 
			{
				try 
				{
					Directory.CreateDirectory(fileName.Substring(0,fileName.LastIndexOf("\\")));
					stream = File.CreateText(fileName);
				}
				catch(Exception e) 
				{
					NameValueCollection detail = new NameValueCollection(2);
					detail.Add("filename", fileName);
					detail.Add("directory", fileName.Substring(0,fileName.LastIndexOf("\\")));
					throw new LoggableException(e, detail);
				}
			}
			catch
			{	//we probably got here because ASP.NET has compiled new code and is launching
				//a new application context(the one we are in now). The former app with the old code
				//will continue to run until it has serviced it's request. It will also,
				//unfortunately, continue to lock the log files. We will log everything to a 
				//stringbuilder until the file becomes available.
				if(buff==null)
					buff = new StringBuilder(5000);				
			}
			if(stream != null) 
			{
				if(buff != null) 
				{
					stream.WriteLine(buff.ToString());
                    stream.Flush();
					buff = null;
				}

                // VISTA has issues in determining running file size and watchers on them.

                //if(watcher != null)
                //    watcher.Dispose();
                //watcher = new FileSystemWatcher();
                //int idx = fileName.LastIndexOf("\\");
                //watcher.Path = fileName.Substring(0,idx);
                //watcher.NotifyFilter = NotifyFilters.Size;
                //watcher.Filter = fileName.Substring(idx + 1);

                //// Add event handlers.
                //watcher.Changed += new FileSystemEventHandler(OnFileSizeChanged);

                //// Begin watching.
                //watcher.EnableRaisingEvents = true;
			}
		}

        private void OnTimeout(Object o)
        {
            OnFileSizeChanged(null, null);
        }

        private void OnFileSizeChanged(object source, FileSystemEventArgs e)
		{
            if(stream==null)
				return;

			//if the file has grown large, archive
			FileInfo file = new FileInfo(fileName);
			lock(this) 
			{
                if(file.Length > fileSize) 
				{
					stream.Close();
					stream = null;
					if(File.Exists(fileName + "-" + archiveIndex.ToString()))
						File.Delete(fileName + "-" + archiveIndex.ToString());
					file.MoveTo(fileName + "-" + archiveIndex.ToString());
					++archiveIndex;
					if(archiveIndex > archiveLimit)
						archiveIndex = 1;
					AttemptStream();
				}
			}
		}

		private void Close() 
		{
			if(stream != null)
				stream.Close();
			stream = null;

			if(watcher != null)
				watcher.Dispose();
			watcher = null;

			GC.SuppressFinalize(this);
		}

		~ LogWriter() 
		{
			Close();
		}

		public static void ShutDown()
		{
			ShutDown(null, null);
		}

		public static void ShutDown(Object sender, EventArgs args)
		{
			activityTimer = null;
			lock(typeof(LogWriter)) 
			{
				ICollection coll = logs.Values;
				foreach(LogWriter log in coll) 
					log.Close();
				logs.Clear();
			}
		}

		private static void CheckLogActivity(Object o) 
		{
			ArrayList closedLogs = new ArrayList();

			lock(typeof(LogWriter)) 
			{
				ICollection coll = logs.Values;
				foreach(LogWriter log in coll) 
				{ 
					if(log.lastWrite.AddMinutes(1) < DateTime.Now) 
					{
						log.Close();
						closedLogs.Add(log.name);
					}
				}
			}

			foreach(String logName in closedLogs)
				logs.Remove(logName);
		}

		/// <summary>
		/// Writes a message to the default log
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="msg">String message to log</param>
		public static void Write(LogLevel level, String msg) 
		{
			LogWriter.Write(level, null, null, msg, null);
		}

		/// <summary>
		/// Logs a message to the specified log
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="msg">String message to log</param>
		public static void Write(LogLevel level, String logName, String msg) 
		{
			LogWriter.Write(level, logName, null, msg, null);
		}

		/// <summary>
		/// Logs a message to the specified log specifying an owner name
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		public static void Write(LogLevel level, String logName, String owner, String msg) 
		{
			LogWriter.Write(level, logName, owner, msg, null);
		}

		/// <summary>
		/// Logs a message to the specified log specifying an owner name
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		/// <param name="writeToEventLog">true if data will also be written to Windows EventLog</param>
		public static void Write(LogLevel level, String logName, String owner, String msg, bool writeToEventLog) 
		{
			LogWriter.Write(level, logName, owner, msg, null, writeToEventLog);
		}

		/// <summary>
		/// Logs a message to the default log with detailed information
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="msg">String message to log</param>
		/// <param name="detail">Name/Value detailes to be logged with message</param>
		public static void Write(LogLevel level, String msg, NameValueCollection detail) 
		{
			LogWriter.Write(level, null, null, msg, detail);
		}

		/// <summary>
		/// Logs a message to the specified log with detailed information
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		/// <param name="detail">Name/Value detailes to be logged with message</param>
		public static void Write(LogLevel level, String logName, String owner, String msg, NameValueCollection detail) 
		{
			LogInfo inf =  new LogInfo(level, logName, owner, msg, detail, false);
			ThreadPool.QueueUserWorkItem(new WaitCallback(PoolWrite), inf);

		}

		/// <summary>
		/// Logs a message to the specified log with detailed information
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		/// <param name="detail">Name/Value detailes to be logged with message</param>
		/// <param name="writeToEventLog">true if data will also be written to Windows EventLog</param>
		public static void Write(LogLevel level, String logName, String owner, String msg, NameValueCollection detail, bool writeToEventLog) 
		{
			LogInfo inf =  new LogInfo(level, logName, owner, msg, detail, writeToEventLog);
			ThreadPool.QueueUserWorkItem(new WaitCallback(PoolWrite), inf);
		}
		/// <summary>
		/// Logs a message to the specified log with detailed information
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		/// <param name="detail">Name/Value detailes to be logged with message</param>
		public static void PoolWrite(object info) 
		{
			LogInfo inf = (LogInfo)info;
			LogLevel level = inf.Level;
			string logName = inf.LogName;
			string owner = inf.Owner;
			string msg = inf.Message;
			NameValueCollection detail = inf.Detail;
			bool writeToEventLog = inf.WriteToEventLog;

			if(logName == null)
				logName = "default";

			//make sure log has been instantiated
			LogWriter log = (LogWriter)logs[logName];
			if(log==null) 
			{
				lock(typeof(LogWriter)) 
				{
					//one more check to prevent queued threads from creating multiple instances
					log = (LogWriter)logs[logName];
					if(log==null) 
					{
						log = new LogWriter(logName);

						//bail if app did not set up config correctly
						if(log.fileName == null)
							return;

						logs.Add(logName, log);
						if(activityTimer == null)
							activityTimer = new Timer(new TimerCallback(CheckLogActivity),null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
					}
				}
			}
			if (writeToEventLog)
				WriteEventLog(level, logName, owner, msg, detail);
			log.Write(level, owner, msg, detail);
		}


		/// <summary>
		/// Logs a message to the Windows Application EventLog 
		/// </summary>
		/// <param name="level">Log level of message</param>
		/// <param name="logName">Name of the log to log to</param>
		/// <param name="owner">Owner name</param>
		/// <param name="msg">String message to log</param>
		/// <param name="detail">Name/Value detailes to be logged with message</param>
		/// <param name="writeToEventLog">true if data will also be written to Windows EventLog</param>
		/// <remarks>Written as an error type entry always with eventid 100, category 1.
		/// If owner was specified it will be used as event source, otherwise event source will be null. This feature should
		/// only be used to log critical error information</remarks>
		public static void WriteEventLog(LogLevel level, String logName, String owner, String msg, NameValueCollection detail) 
		{
			// Note that mapping of old log info to eventlog not exact:
			//	Owner, if present becomes the event source
			//	EventId is always 100
			//	Event Category is always 1
			//	NameValueCollection is assumed to be strings without keys and each item is suffixed with new line
			// *** Use this feature only to write critical error information ***

			byte[] detailbytes = null;
			if (detail != null)
			{
				int size = 0;
				foreach(string item in detail)
					size += (item.Length + 1);

				StringBuilder sb = new StringBuilder(size);

				foreach( string item in detail)
					sb.Append(item + "\n");

				System.Text.UTF8Encoding utf8 = new UTF8Encoding();
				detailbytes = utf8.GetBytes(sb.ToString());
			}

			WriteEventLog(msg, "Application", owner, EventLogEntryType.Error, 100, 0, detailbytes);

		}

		/// <summary>
		/// Write to the Windows EventLog
		/// </summary>
		/// <param name="msg">the message to be logged</param>
		/// <param name="logName">the name of the EventLog being written</param>
		/// <param name="eventSource">Event Source</param>
		/// <param name="type">EventLogEntryType value</param>
		/// <param name="eventId">id of the event to be written</param>
		/// <param name="category">category of the event to be written</param>
		/// <param name="details">Additional array of bytes being written</param>
		public static void WriteEventLog(string msg, string logName, string eventSource, EventLogEntryType type, int eventId, short category, byte[] details)
		{
			EventLog el = null;
			if (eventSource == null)
			{
				el = new EventLog(logName, ".");
			}
			else
			{
				// Create the source, if it does not already exist.
				if(!EventLog.SourceExists(eventSource))
					eventSource = "null";

				el = new EventLog(logName, ".", eventSource);
			}

			if (details != null)
				el.WriteEntry(msg, type, eventId, category, details);
			else
				el.WriteEntry(msg, type, eventId, category);
		}

		public static int FileSize
		{
			set { fileSize = value; }
			get { return fileSize; }
		}

	}
}
