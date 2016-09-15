using System;
using System.IO;

namespace yellowx.Framework.Logging
{
    public class LogWriterConfig
    {
        internal const int LOG_FILE_SIZE = 100;//log to another file after 50Kbytes
        internal const int LOG_ITEM_SIZE = 20;
        internal const string LOG_FILE_EXTENSION = ".log";
        internal const string LOG_FILE_NAME = "log";

        private readonly int logSize;//maximum size of file to roll.
        private readonly int itemSize;//maximum number of item to write to file.
        private readonly string path;// the path to stored log files.
        private readonly string fileName;//the file name of the current log file.
        private readonly string fileExtension;//the file name of the current log file.
        private readonly string appName;//the application create this log.

        internal int LogSize { get { return logSize * 1024; } }
        internal int ItemSize { get { return itemSize; } }
        internal string ApplicationName { get { return appName ?? Configuration.ApplicationName; } }

        public string FullFilePath
        {
            get
            {
                var fileExtension = EnsureFileExtension(this.fileExtension);
                return Path.Combine(path, fileName + DateTime.Now.ToString("-yyMMdd-hhmm") + this.fileExtension);
            }
        }

        public LogWriterConfig(string path) : this(null, path)
        {

        }
        public LogWriterConfig(string applicationName, string path) : this(applicationName, path, LOG_FILE_SIZE, LOG_ITEM_SIZE)
        {

        }

        public LogWriterConfig(string applicationName, string path, int logSize, int itemSize)
            : this(applicationName, path, LOG_FILE_NAME, LOG_FILE_EXTENSION, logSize, itemSize)
        { }

        public LogWriterConfig(string applicationName, string path, string fileName, string fileExtension, int logSize, int itemSize)
        {
            appName = applicationName;
            this.path = path;
            this.fileName = fileName;
            this.fileExtension = fileExtension;
            this.logSize = logSize;
            this.itemSize = itemSize;
            Initialize();
        }
        private void Initialize()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private string EnsureFileExtension(string fileExtension)
        {
            return fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension;
        }
    }
}
