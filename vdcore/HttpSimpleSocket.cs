using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Threading;
using System.Net.Security;

namespace Vendare.Utils
{
	/// <summary>
	/// This class provides a simplified Http interface as an alternative to
	/// the stock .NET HttpWebRequest class. Applications should use this class 
	/// when they must rely heavily on http communication to other servers. 
	/// If an app does not make frequent and wide spread use of http communication
	/// it is recommended that the app continue to use the HttpWebRequest class.
	/// </summary>
	public class HttpSimpleSocket
	{
		private string host = null;
		private int port = 80;
		private string path = "";
		private string origUrl = null;
		private TcpClient client = new TcpClient();
		private int connTO = 0;
		private object syncObj = new object();
		private Exception connException = null;

        private byte[] buff;
        private string body = null;

        private bool timedOut = false;
        public bool useSsl = false;

		/// <summary>
		/// Instantiates a new socket, attempts to connect and retrieves the response body.
		/// The body can then be retrieved using the Body property.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="connectionTimeout"></param>
		/// <param name="readTimeout"></param>
		public HttpSimpleSocket(string url, int connectionTimeout, int readTimeout):
			this(url, connectionTimeout, readTimeout, System.Web.HttpContext.Current)
		{
		}
         
		/// <summary>
		/// Instantiates a new socket, attempts to connect and retrieves the response body.
		/// The body can then be retrieved using the Body property.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="connectionTimeout"></param>
		/// <param name="readTimeout"></param>
        public HttpSimpleSocket(string url, int connectionTimeout, int readTimeout, System.Web.HttpContext context)
		{
            Initialize(url, connectionTimeout, readTimeout);

            try
            {
                GetBody(context);
            }
            finally
            {
                client.Close();
            }
        }

        private void GetBody(System.Web.HttpContext context)
        {
            CustomTracer.Write("HttpSimpleSocket GetBody: establishing a TcpClient connection", context);
            GetConnection();
            CustomTracer.Write("HttpSimpleSocket GetBody: got a connection", context);

            string request =
                "GET " + path + " HTTP/1.1\r\n" +
                "Host: " + host + ":" + port + "\r\n\r\n" +
                "Connection : close\r\n\r\n";

            GetResponse(context,request );
            ExtractResponse();

            CustomTracer.Write("HttpSimpleSocket GetBody: done reading response", context);
		}

        private void ExtractResponse () 
        {
            //read past the headers
            int idx = body.IndexOf("\r\n\r\n");
            int start = idx + 4;

            if (idx == -1)
            {
                //can't find delimiter between headers and body. It is possible but not legal that no
                //headers were sent. Try to treat the entire response as the body.
                start = 0;
                LogWriter.Write(LogLevel.Debug, "Can't find Body:" + body);
            }

            StringBuilder bodyBuff = new StringBuilder();

            int lenIdx = body.ToLower().IndexOf("chunked", 0, start);
            if (lenIdx > -1) //chunked response
            {
                string chSize = null;
                int ich = 0;
                while (true)
                {
                    //find chunk size
                    idx = body.IndexOf("\r\n", start);
                    if (idx == -1)
                        throw new ApplicationException("could not find the chunk size");
                    chSize = body.Substring(start, idx - start);
                    try
                    {
                        ich = Int32.Parse(chSize.Trim(), System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    catch (FormatException)
                    {
                        throw new ApplicationException("Invalid Chunk Size");
                    }
                    if (ich == 0)
                        break;
                    start = idx + 2;
                    //parse chunk
                    //					bodyBuff.Append(body.Substring(start,ich));
                    bodyBuff.Append(Encoding.UTF8.GetString(buff, start, ich));
                    start += ich + 2;
                }
                body = bodyBuff.ToString();
            }
            else //non-chunked
            {
                //extract length
                lenIdx = body.ToLower().IndexOf("content-length:", 0, start);
                if (lenIdx == -1)	//no content-length header, just grab everything after the headers
                    body = Encoding.UTF8.GetString(buff, start, buff.Length - start);
                else	//use the content-length header
                {
                    lenIdx += "content-length:".Length;
                    int lenEnd = body.IndexOf("\r\n", lenIdx);
                    string len = body.Substring(lenIdx, lenEnd - lenIdx);
                    int ilen = 0;

                    try
                    {
                        ilen = Int32.Parse(len);
                    }
                    catch (FormatException)
                    {
                        throw new ApplicationException("Unable to parse body length");
                    }

                    //					body = body.Substring(start, ilen);
                    body = Encoding.UTF8.GetString(buff, start, ilen);
                }
            }
        }

        //grab the entire raw response
        //this may not be the best way to do this but the response is puuled in two formats:
        //ASCII and binary. We use the ascii copy to search the headers and chunks. Some utf-8
        //text may get mangled but atleast the character count is preserved. If we converted to
        //UTF-8, the text would look good but some multi-byte characters would distort the character
        //count. We use the binary data to parse out the needed chunks and then convert that to 
        //UTF-8 which gets passed back to the client.
        private void GetResponse(System.Web.HttpContext context, string strReq)
        {
            NetworkStream ns = client.GetStream();

            Stream stream = null;
            if (useSsl)
            {
                var sslStream = new SslStream(ns);
                sslStream.AuthenticateAsClient(host);
                stream = sslStream;
            }
            else
                stream = new BufferedStream(ns);

            CustomTracer.Write("HttpSimpleSocket GetBody: got a response",context);

            ArrayList lst = new ArrayList();
            StringBuilder strBuff = new StringBuilder();

            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strReq);
                stream.Write(bytes, 0, bytes.Length);

                buff = new byte[1024];
                int bytesRead = 0;

                try
                {
                    while ((bytesRead = stream.Read(buff, 0, buff.Length)) > 0)
                    {
                        byte[] cpy = new byte[bytesRead];
                        Array.Copy(buff, cpy, bytesRead);

                        lst.AddRange(cpy);
                        strBuff.Append(Encoding.ASCII.GetString(buff, 0, bytesRead));
                    }
                }
                catch (System.IO.IOException ioe)
                {
                    Exception inner = ioe.InnerException;
                    if (inner != null && inner is SocketException)
                        throw ioe.InnerException;
                    else
                        throw ioe;
                }

                buff = (byte[])lst.ToArray(Type.GetType("System.Byte"));
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                client.Close();
            }

            body = strBuff.ToString();
        }

        private void GetConnection()
        {
            if (connTO < 1)	//no timeout so open in this thread
            {
                Connect();
            }
            else //open in separate thread and shut it down if too slow
            {
                //acquire initial lock
                Monitor.Enter(syncObj);
                Thread t = new Thread(new ThreadStart(Connect));
                t.Start();
                //ThreadPool.QueueUserWorkItem(new WaitCallback(Connect));

                //release lock to connection thread and wait
                bool success = Monitor.Wait(syncObj, connTO);

                if (success)
                {
                    Monitor.Exit(syncObj);
                    if (connException != null)
                    {
                        client.Close();
                        throw connException;
                    }
                }
                else
                {
                    timedOut = true;
                    t.Abort();
                    client.Close();
                    throw new ApplicationException("Unable to connect within timeout.");
                }
            }
        }

        private void Connect() 
		{
			if(timedOut)
				return;

			Monitor.Enter(syncObj);
			try 
			{
				connException = null;
				client.Connect(host, port);
			}
			catch(Exception e) 
			{
				connException = e;
			}
			finally
			{
				Monitor.Pulse(syncObj);
				Monitor.Exit(syncObj);
			}
		}

        //private void initHttpSimpleSocket(string url, int connectionTimeout, int readTimeout)
        //{
        //    if (url == null)
        //        throw new ApplicationException("URL Cannot be Null");
        //    connTO = connectionTimeout;
        //    client.ReceiveTimeout = readTimeout;

        //    origUrl = url;
        //    if (url.ToLower().StartsWith("http://"))
        //        host = url.Substring(7);
        //    else
        //        host = url;

        //    int idx = host.IndexOf("/");
        //    if (idx == -1)
        //        idx = host.Length;
        //    if (idx == 0)
        //        throw new ApplicationException("Invalid Url: " + url);

        //    // set path to everything from / to the end
        //    if (idx < host.Length - 1)
        //        path = host.Substring(idx);

        //    //set host to everything up to /
        //    host = host.Substring(0, idx);

        //    // determine if port is specified
        //    idx = host.IndexOf(":");
        //    if ((idx > 0))
        //    {
        //        // set port to everything after :
        //        port = Int32.Parse(host.Substring(idx + 1));

        //        // set host to everything up to :
        //        host = host.Substring(0, idx);
        //    }
        //}

        private void Initialize(string url, int connectionTimeout, int readTimeout)
        {
            connTO = connectionTimeout;
            client.ReceiveTimeout = readTimeout;

            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.CurrentCultureIgnoreCase))
                    useSsl = true;
                host = uri.Host;
                path = uri.PathAndQuery;
                if (uri.Port > 0)
                    port = uri.Port;
            }
            else
                throw new Exception("Invalid Url: " + url);
        }

		public string Body 
		{
			get { return body;}
		}
	}
}
