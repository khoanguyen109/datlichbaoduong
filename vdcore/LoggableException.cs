using System;
using System.Web;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Vendare.Utils;

namespace Vendare.Error
{
	/// <summary>
	/// Provides an exception that logs to a file and/or database.
	/// </summary>
	/// <remarks>
	/// This class shoulds be used when catching and passing application exceptions.
	/// The class accepts the instance of the caught exception and logs importat
	/// information regarding the exception and the environment in which the exception 
	/// occured. Using this class, try...catch blocks can be created that will simply 
	/// rethrow a caught LoggableException and handle other exceptions and then throw
	/// the created Loggable exception.
	/// The class can be configured by setting up settings in the appSettings portion
	/// of web.config.
	/// logErrorsToDB: if 'y', errors will be logged in the errors table of the DB
	/// logErrorsToFile: if 'y', errors will be logged to a file.
	/// errorFile: absolute path of file to log errors to.
	///</remarks>
	[Serializable]
	public class LoggableException : ApplicationException
	{
		private static bool logToDB = false;
		private static bool logToFile = false;
		private static String logFile = null;
		private static String dsn = null;
		private static String appName = null;
		private static StreamWriter file = null;
		private String mAppName = null;
		private String source = null;
		private String trace = null;
		private String errorInfo = null;
		private StringBuilder propertiesBuffer = new StringBuilder();
		private String message = null;
		private static String database = "jackpot";
		private static FileSystemWatcher watcher;
		private static int archiveIndex = 1;
		private static int fileSize = 1048576;
		private static int archiveLimit = 5;
		private NameValueCollection detail = null;
		private static int backoffTime = 5;
		private static DateTime resumeDBLogging;
		private static String[,] withApos = { {"&","&amp;"},{"<","&lt;"},{">","&gt;"}, {"\"","&quot;"},{"\'","&apos;"} };
		private static String[,] noApos = { {"&","&amp;"},{"<","&lt;"},{">","&gt;"}, {"\"","&quot;"}};
        private static object syncObject = new object();
		//private static Mutex mut = null;
		//public static StringBuilder debugString = new StringBuilder();

		/// <summary>
		/// Initializes static variables and sets up the text file for logging.
		/// </summary>
		static LoggableException()
		{
			try 
			{
				//System.Diagnostics.Debugger.Launch();
				resumeDBLogging = DateTime.Now;
				//Retrieve the database backoff time.  If there is an error
				//writing to the db then LoggableException will back off
				//for the speficied number of minutes before trying again.
				try
				{
					backoffTime = Int32.Parse(ConfigurationSettings.AppSettings.Get("backoffTime"));
				}
				catch
				{
					backoffTime = 5;
				}
				//debugString.Append("backoffTime=" + backoffTime + "<br>");
				if(ConfigurationSettings.AppSettings.Get("errorDB") != null)
					database = ConfigurationSettings.AppSettings.Get("errorDB");
				String configVal = ConfigurationSettings.AppSettings.Get("logErrorsToDB");
				if(configVal !=null && configVal.Equals("y"))
					logToDB = true;
				configVal = ConfigurationSettings.AppSettings.Get("logErrorsToFile");
				if(configVal !=null && configVal.Equals("y"))
					logToFile = true;
				logFile = ConfigurationSettings.AppSettings.Get("errorFile");
                /*
				try 
				{
					mut = new Mutex(false, logFile.Replace("\\","|"));
				}
				catch(Exception e1) 
				{
					LogWriter.Write(LogLevel.Info, "Error with mutex::" + logFile + "::" + e1.ToString());
					throw e1;
				}
                 */
				dsn = ConfigurationSettings.AppSettings.Get("dsn");
				appName = ConfigurationSettings.AppSettings.Get("appName");

				if(!logToFile)
					return;
			
				try 
				{
					archiveLimit = Int32.Parse(ConfigurationSettings.AppSettings.Get("errorArchiveLimit"));
				}
				catch {}
				try 
				{
					fileSize = Int32.Parse(ConfigurationSettings.AppSettings.Get("errorLogFileSize"));
				}
				catch {}

				AppDomain.CurrentDomain.DomainUnload += new EventHandler(LoggableException.ShutDown);
				AppDomain.CurrentDomain.ProcessExit += new EventHandler(LoggableException.ShutDown);

                /*
				try 
				{
					LoggableException.openFile();
				}
				catch(Exception e2) 
				{
					LogWriter.Write(LogLevel.Info, "Error with openFile::" +  logFile);
					LogWriter.Write(LogLevel.Info, e2.ToString());
					throw e2;
				}
                 */

                if(!Directory.Exists(logFile.Substring(0,logFile.LastIndexOf("\\"))))
                    Directory.CreateDirectory(logFile.Substring(0,logFile.LastIndexOf("\\")));

				watcher = new FileSystemWatcher();
				int idx = logFile.LastIndexOf("\\");
				watcher.Path = logFile.Substring(0,idx);
				watcher.NotifyFilter = NotifyFilters.Size;
				watcher.Filter = logFile.Substring(idx + 1);

				// Add event handlers.
				watcher.Changed += new FileSystemEventHandler(OnFileSizeChanged);

				// Begin watching.
				watcher.EnableRaisingEvents = true;

			}
			catch(Exception e)
			{
				try
				{
					LogWriter.Write(LogLevel.Info, e.ToString());
				}
				catch {}
				throw e;
			}
		}

        /*
		private static void openFile()
		{
			//append to existing file and create new file if it does not exist.
			try 
			{
				file = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 512, false));
			}
			catch(DirectoryNotFoundException) 
			{
				Directory.CreateDirectory(logFile.Substring(0,logFile.LastIndexOf("\\")));
				file = new StreamWriter(new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 512, false));
			}
		}
         */

		private static void OnFileSizeChanged(object source, FileSystemEventArgs e)
		{
			//if the file has grown large, archive
			FileInfo fileInfo = new FileInfo(logFile);

			if (fileInfo.Exists)
			{
				lock (syncObject)
				{
					if (fileInfo.Exists)
					{
						if (fileInfo.Length > fileSize)
						{
							try
							{
								if (File.Exists(logFile + "-" + archiveIndex.ToString()))
									File.Delete(logFile + "-" + archiveIndex.ToString());
								fileInfo.MoveTo(logFile + "-" + archiveIndex.ToString());
							}
							catch
							{
								LogWriter.Write(LogLevel.Info, e.ToString());
							}
							++archiveIndex;
							if (archiveIndex > archiveLimit)
								archiveIndex = 1;

							watcher.Dispose();
							watcher = new FileSystemWatcher();
							int idx = logFile.LastIndexOf("\\");
							watcher.Path = logFile.Substring(0, idx);
							watcher.NotifyFilter = NotifyFilters.Size;
							watcher.Filter = logFile.Substring(idx + 1);

							// Add event handlers.
							watcher.Changed += new FileSystemEventHandler(OnFileSizeChanged);

							// Begin watching.
							watcher.EnableRaisingEvents = true;
						}
					}
				}
			}
		}

		private static void ShutDown(Object sender, EventArgs args) 
		{
            /*
			if(file != null) 
			{
				file.Flush();
				file.Close();
				file = null;
			}
             */

			if(watcher != null)
				watcher.Dispose();
			watcher = null;

		}

		/// <summary>
		/// Creates a Loggable exception from an existing exception.
		/// </summary>
		/// <param name="e">Existing exception which will become the inner exception</param>
		/// <param name="detail">Name-value pairs describing execution info.</param>
		public LoggableException(Exception e, NameValueCollection detail):base("",e)
		{
			this.detail = detail;
			LogError(FormatDetail(detail, null));
		}

		public LoggableException(String appName, Exception e, NameValueCollection detail):base("",e)
		{
			this.detail = detail;

			mAppName = appName;
			LogError(FormatDetail(detail, null));
		}

		/// <summary>
		/// Creates a Loggable exception from an existing exception.
		/// </summary>
		/// <param name="e">Existing exception which will become the inner exception</param>
		/// <param name="detail">Name-value pairs describing execution info.</param>
		/// <param name="request">HttpRequest of ASP page for extended logging of page info.</param>
		public LoggableException(Exception e, NameValueCollection detail, HttpRequest request):base("",e)
		{
			this.detail = detail;
			LogError(FormatDetail(detail, request));
		}

		public LoggableException(String appName, Exception e, NameValueCollection detail, HttpRequest request):base("",e)
		{
			this.detail = detail;
			mAppName = appName;
			LogError(FormatDetail(detail, request));
		}

		/// <summary>
		/// Creates a custom Loggable exception. Raised by calling application.
		/// </summary>
		/// <param name="message">Error Message to give to exception</param>
		/// <param name="detail">Name-value pairs describing execution info.</param>
		public LoggableException(String message, NameValueCollection detail):base(message)
		{
			this.detail = detail;
			LogError(FormatDetail(detail, null));
		}

		public LoggableException(String appName, String message, NameValueCollection detail):base(message)
		{
			this.detail = detail;
			mAppName = appName;
			LogError(FormatDetail(detail, null));
		}

		protected void LogError(String detail) 
		{
			bool success = true;
			errorInfo = CreateErrorString(detail);

			//log to the DB if specified in config or if the errorFile is null
			//and if the backoff time has expired.
			//debugString.Append("LogError: resumeDBLogging=" + resumeDBLogging + "<br>");
			if (logToDB && resumeDBLogging <= DateTime.Now)
			{
				//debugString.Append("LogError: Attempt write to db.<br>");
				SqlConnection conn = null;
				success = false;
				if(mAppName == null)
					mAppName = appName;

				try 
				{
					String message;
					String type;

					//if this is a custom error, there will be no inner exception
					if(base.InnerException != null) 
					{
						message = base.InnerException.GetType() + ":" + base.InnerException.Message;
						type = base.InnerException.GetType().ToString();
					}
					else 
					{
						message = base.Message;
						type = this.GetType().ToString();
					}

					conn = new SqlConnection(dsn);
					conn.Open();
					SqlCommand command = new SqlCommand(database + ".dbo.sp_insert_error", conn);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add("@Message", SqlDbType.VarChar).Value = message;
					command.Parameters.Add("@Type", SqlDbType.VarChar).Value = type;
					command.Parameters.Add("@Number", SqlDbType.Char).Value = "0";
					command.Parameters.Add("@Source", SqlDbType.VarChar).Value = source;
					command.Parameters.Add("@Paramaters", SqlDbType.Text).Value = detail;
					command.Parameters.Add("@Thread", SqlDbType.Int).Value = 0;
					command.Parameters.Add("@Server", SqlDbType.VarChar).Value = mAppName + ":" + SystemInformation.ComputerName;
					command.ExecuteNonQuery();
					success = true;
				}
				catch(Exception e) 
				{
					//LogWriter.Write(LogLevel.Info, e.ToString());
					resumeDBLogging = DateTime.Now.AddMinutes(backoffTime);
					success = false;
				}
				finally
				{
					if(conn != null)
						conn.Close();
				}
			}

			//if unsuccessfull, logs to file
			if(logToFile || !success) 
			{
                lock (syncObject)
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(logFile,
                                   FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            using (StreamWriter SW = new StreamWriter(fileStream, Encoding.UTF8))
                            {

                                SW.WriteLine(errorInfo);
                                SW.Flush();
                            }
                        }
                    }
                    catch (Exception ex)
                    { LogWriter.Write(LogLevel.Info, ex.ToString()); }
                }
			}
			
		}


		/// <summary>
		/// Formats the details surrounding the exception into an xml string
		/// </summary>
		protected String FormatDetail(NameValueCollection detail, HttpRequest request) 
		{
			StackTrace fullTrace = new StackTrace(2,true);
			trace = createStackTrace(fullTrace);
			if(request != null)
				source = request.Url.ToString();
			else 
			{
				if(base.InnerException != null && base.InnerException.StackTrace != null)
				{
					source = base.InnerException.StackTrace;
					source = source.Substring(source.LastIndexOf("at ")+3);
				}
				else 
				{
					source = fullTrace.ToString();
					source = source.Substring(source.IndexOf("at ")+3, source.IndexOf(")") - source.IndexOf("at ")-2);
				}
			}

			StringBuilder buf = new StringBuilder(1000);
			buf.Append("<details>\r\n");
			
			if(detail != null) 
			{
				propertiesBuffer.Append("<properties>\r\n");
				FormatNameValues("property", propertiesBuffer, detail);
				propertiesBuffer.Append("</properties>\r\n");
			}
			buf.Append(propertiesBuffer);

			if(request != null)
				FormatRequest(buf, request);
    
			buf.Append("<trace>\r\n");
			buf.Append(SafeXMLString(trace));
			buf.Append("\r\n</trace>\r\n");
			buf.Append("</details>\r\n");
			return buf.ToString();
		}

		/// <summary>
		/// Formats HttpRequest attributes surrounding exception into an xml string
		/// </summary>
		private void FormatRequest(StringBuilder buf, HttpRequest request) 
		{
			//record certain server variable info
			buf.Append("<ServerVariables>\r\n");
			FormatNameValues("ServerVariable", buf, request.ServerVariables, "REMOTE_ADDR");
			FormatNameValues("ServerVariable", buf, request.ServerVariables, "URL");
			FormatNameValues("ServerVariable", buf, request.ServerVariables, "HTTP_HOST");
			FormatNameValues("ServerVariable", buf, request.ServerVariables, "HTTP_USER_AGENT");
			FormatNameValues("ServerVariable", buf, request.ServerVariables, "HTTP_REFERER");

			buf.Append("</ServerVariables>\r\n");
    
			//record querystring and form values
			buf.Append("<post-get>\r\n");
			FormatNameValues("parameter", buf, request.Form);
			FormatNameValues("parameter", buf, request.QueryString);
			buf.Append("</post-get>\r\n");

			//record cookies
			buf.Append("<cookies>\r\n");
			String[] keys = request.Cookies.AllKeys;
			for(int i=0; i < keys.Length; i++) 
			{
				buf.Append("\t<cookie");
				buf.Append(" key=\"");
				buf.Append(SafeXMLString(keys[i].Trim()));
				buf.Append("\" value=\"");
				buf.Append(SafeXMLString(request.Cookies.Get(keys[i]).Value));
				buf.Append("\" />\r\n");
			}
			buf.Append("</cookies>\r\n");

			//record the html error string
			if(base.InnerException is HttpException) 
			{
				buf.Append("<htmlMessage>\r\n");
				buf.Append(SafeXMLString(((HttpException)base.InnerException).GetHtmlErrorMessage()));
				buf.Append("</htmlMessage>\r\n");
			}

			//record parse exception stupp
			if(base.InnerException is HttpParseException) 
			{
				HttpParseException parseException = (HttpParseException)base.InnerException;
				buf.Append("<fileName>");
				buf.Append(parseException.FileName);
				buf.Append("</fileName>\r\n");
				buf.Append("<lineNumber>");
				buf.Append(parseException.Line);
				buf.Append("</lineNumber>\r\n");
			}
		}

		private void FormatNameValues(String name, StringBuilder buf, NameValueCollection col) 
		{
			String[] keys = col.AllKeys;
			for(int i=0; i < keys.Length; i++) 
			{
				//skip any internal asp hidden fields
				if(keys[i].StartsWith("__"))
					continue;

				buf.Append("\t<");
				buf.Append(name);
				buf.Append(" key=\"");
				buf.Append(SafeXMLString(keys[i].Trim()));
				buf.Append("\" value=\"");
				buf.Append(SafeXMLString(col.Get(keys[i])));
				buf.Append("\" />\r\n");
			}
		}

		private void FormatNameValues(String name, StringBuilder buf, NameValueCollection col, String index) 
		{
			buf.Append("\t<");
			buf.Append(name);
			buf.Append(" key=\"");
			buf.Append(SafeXMLString(index));
			buf.Append("\" value=\"");
			buf.Append(SafeXMLString(col[index]));
			buf.Append("\" />\r\n");
		}

		/// <summary>
		/// Transforms complete error information to a string used for the log file
		/// </summary>
		public String CreateErrorString(String detail) 
		{
			StackTrace t = new StackTrace();
			
			StringBuilder buf = new StringBuilder(2000);
			DateTime date = DateTime.Now;

			buf.Append("[");
			buf.Append(date);
			buf.Append("]");
			if(base.InnerException != null)
				buf.Append(base.InnerException.GetType());
			else
				buf.Append("CUSTOM ERROR");

			buf.Append("\r\nMESSAGE=\"");
			
			if(base.InnerException != null)
				message = base.InnerException.Message;
			else
				message = base.Message;

			buf.Append(message);

			buf.Append("\"\r\nSOURCE=\"");
			buf.Append(source);
			buf.Append("\"\r\n");
			buf.Append(detail + "\r\n");

			return buf.ToString();
		}

		public override String ToString() 
		{
			return errorInfo;
		}

		private String createStackTrace(StackTrace trace) 
		{
			StringBuilder result = new StringBuilder();
			if(base.InnerException != null)
				result.Append(base.InnerException.StackTrace);

			result.Append("\r\nFull Stack Trace:\r\n");
			int frameCount = trace.FrameCount;
			for(int n = 0; n < frameCount; ++n)
			{
				StringBuilder frameString = new StringBuilder();
				StackFrame frame = trace.GetFrame(n);

				int lineNumber = frame.GetFileLineNumber();
				string fileName = frame.GetFileName();
				MethodBase methodBase = frame.GetMethod();
				string methodName = methodBase.Name;
				ParameterInfo [] paramInfos = methodBase.GetParameters();

				result.AppendFormat("{0} - line {1}, {2}",
					fileName,
					lineNumber,
					methodName);
				if(paramInfos.Length == 0)
				{
					// No parameters for this method; display
					// empty parentheses.
					result.Append("()\r\n");
				}
				else
				{
					// Iterate over parameters, displaying each parameter’s
					// type and name.
					result.Append("(");
					int count = paramInfos.Length;
					for(int i = 0; i < count; ++i)
					{
						Type paramType = paramInfos[i].ParameterType;
						result.AppendFormat("{0} {1}",
							paramType.ToString(),
							paramInfos[i].Name);
						if(i < count - 1)
							result.Append(",");
					}
					result.Append(")\r\n");
				}
			}

			return result.ToString();
		}

		public String GetMsg()
		{
			return message;
		}

		public NameValueCollection Detail
		{
			get	{	return detail;	}
		}

		public String Properties
		{
			get 
			{
				if(propertiesBuffer != null)
					return propertiesBuffer.ToString();
				else
					return null;
			}
		}

		//Copied the below methods (SafeXMLString, IsEmpty) from the Helper class.  The Helper class
		//has a static constructor that makes a call back to LoggableException when an
		//error occurs, potentially beginning an endless loop.

		/// <summary>
		/// Converts a string into an XML legal string by conducting entity substitutions
		/// </summary>
		/// <param name="text">Original string to convert</param>
		/// <returns>XML Legal String</returns>
		static public String SafeXMLString(String text) 
		{
			return SafeXMLString(text,false);
		}

		/// <summary>
		/// Converts a string into an XML legal string by conducting entity substitutions,
		/// which can be displayed in HTML by nonsubstituting apostrophe entities.
		/// </summary>
		/// <param name="text">Original string to convert</param>
		/// <returns>HTML appropriate XML Legal String</returns>
		/// <remarks>This is actually NOT XML legal since the apostrophes are not escaped.
		/// However, HTML would show the apostrophe entity if it was used.
		/// </remarks>
		static public String SafeXMLString(String text, bool ignoreApos) 
		{
			if(IsEmpty(text))
				return text;

			String[,] entities;

			if(!ignoreApos)
				entities = withApos;
			else
				entities = noApos;
	          
			for (int i = 0; i < entities.Length/2; i++) 
			{
				text = text.Replace(entities[i,0], entities[i,1]);
			}

			return text;
		}

		/// <summary>
		/// tests a string to make sure it is not null or empty
		/// </summary>
		/// <param name="value">String to test</param>
		/// <returns>True if the sting is null or is empty or only contains empty spaces</returns>
		public static bool IsEmpty(String value) 
		{
			bool test = false;
        
			if(value==null)
				test = true;
			else if(value.Trim().Equals(""))
				test = true;
        
			return test;
		}
	}
}
