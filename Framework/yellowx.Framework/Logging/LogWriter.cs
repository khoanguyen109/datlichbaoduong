using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using yellowx.Framework.Extensions;
using System.Collections.Concurrent;
using yellowx.Framework.IO;

namespace yellowx.Framework.Logging
{
    public interface ILog
    {
        void Log(string message);
        void Log(string message, System.Exception exception);
        void Log(string message, StringDictionary details);
        void Log(string message, System.Exception exception, StringDictionary details);
    }
    public class LogWriter : Object, ILog
    {
        private readonly LogWriterConfig config;
        private readonly ConcurrentQueue<LogItem> itemQueue = new ConcurrentQueue<LogItem>();
        private Thread processQueueThread;
        private DateTime lastLogDate;
        private volatile bool isRunning;

        public bool IsRunning { get { return isRunning; } }

        public LogWriter(LogWriterConfig config)
        {
            Assert.CannotNull(config, "Unable to set NULL LogWriterConfig item");
            this.config = config;
        }


        #region Implements ILog interface
        public virtual void Log(string message)
        {
            Log(message, null, null);
        }
        public virtual void Log(string message, StringDictionary details)
        {
            Log(message, null, details);
        }
        public virtual void Log(string message, System.Exception exception)
        {
            Log(message, exception, null);
        }
        public virtual void Log(string message, System.Exception exception, StringDictionary details)
        {
            itemQueue.Enqueue(new LogItem(message, exception, details));
            if (!isRunning)
            {
                lock (this)
                {
                    if (!isRunning)
                    {
                        isRunning = true;
                        processQueueThread = new Thread(ProcessQueue);
                        processQueueThread.Start();
                    }
                }
            }
        }
        /// <summary>
        /// Shuts down the log write and write all current logs to file.
        /// </summary>
        public void ShutDown()
        {
            if (isRunning)
            {
                if (processQueueThread != null)
                {
                    processQueueThread.Join();
                    processQueueThread = null;
                }
                isRunning = false;
            }
        }
        #endregion

        #region Private methods     
        private void ProcessQueue(object state)
        {
            var isProcessing = true;
            while (isProcessing)
            {
                var item = Default<LogItem>();
                if (itemQueue.TryDequeue(out item))
                {
                    WriteLog(config.FullFilePath, item.ToString());
                    lastLogDate = DateTime.Now;
                }
                //thread stopped if nothing in queue for 2 minutes.
                if (itemQueue.Count == 0)
                {
                    isProcessing = lastLogDate.AddSeconds(2) > DateTime.Now;//this means we need to wait this logwriter thread stopped???? fixed it.
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            Configuration.WriteDebugView(string.Format("Thread {0} is stopped", Thread.CurrentThread.ManagedThreadId));
            lastLogDate = DateTime.MinValue;
            processQueueThread = null;
            isRunning = false;
        }

        private void WriteLog(string fullLogFile, string message)
        {
            var streamer = new Streamer();

            this.TryCatch(() =>
            {
                using (var writer = new Streamer().AcquireWriter(fullLogFile))
                {
                    writer.WriteLine(message);
                }
            });
        }
        private void RollLog(string fullLogFile, string newFullFileLog)
        {
            this.TryCatch(() =>
            {
                var fileInfo = new FileInfo(fullLogFile);
                if (fileInfo.Exists && fileInfo.Length >= config.LogSize)
                {
                    File.Move(fullLogFile, newFullFileLog);
                    Thread.Sleep(500);//Sleep to make sure the next file will have another timestamp.
                }
            });
        }
        #endregion
    }
}
