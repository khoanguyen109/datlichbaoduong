using System.IO;
using System.Threading;

namespace yellowx.Framework.IO
{
    /// <summary>
    /// A stream handler allows to read/write a file. This handler will retry to read/write file with retry feature.
    /// </summary>
    public class Streamer : Object
    {
        private const int MAX_RETRY = 10;

        public StreamReader AcquireReader(string fullFilePath, int retryTimes = MAX_RETRY)
        {
            var retry = 0;
            while (retry < retryTimes)
            {
                try
                {
                    var streamReader = new StreamReader(fullFilePath);
                    streamReader.BaseStream.Position = 0;
                    return streamReader;
                }
                catch (System.Exception ex)
                {
                    retry++;
                    if (retry > retryTimes)
                        Configuration.WriteEventLogEntry("Unable to acquire reader " + fullFilePath, ex);
                    else
                        Thread.Sleep(500);
                }
            }
            return null;
        }

        public StreamWriter AcquireWriter(string fullFilePath, bool append = true, int retryTimes = MAX_RETRY)
        {
            var retry = 0;
            while (retry < retryTimes)
            {
                try
                {
                    return new StreamWriter(fullFilePath, append);
                }
                catch (System.Exception ex)
                {
                    retry++;
                    if (retry > retryTimes)
                        Configuration.WriteEventLogEntry("Unable to acquire reader " + fullFilePath, ex);
                    else
                        Thread.Sleep(500);
                }
            }
            return null;
        }
    }
}
