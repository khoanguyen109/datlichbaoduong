using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Vendare.Error;

namespace Vendare.Utils
{
	/// <summary>
	/// Encapsulates a querystring and provides typical behaviors
	/// </summary>
	/// <remarks>Note that this class derives from NameValueCollection and therefore inherits NameValueCollection behaviors. 
	/// It makes a querystring convertible with string representations in either name-value-pair or XML formats and also provides 
	/// services for encryption/decryption and XSL transformations.</remarks>
	public class QueryString : NameValueCollection
	{
		#region Data Members
		public enum QueryStringEncoding
		{
			None,
			Html,
			Url
		}
		/// <summary>
		/// Are the values for this QueryString object encrypted or not?
		/// </summary>
		public bool IsEncrypted = false;
		protected int queryStringStorageHighWaterMark = 100;
		protected StringBuilder stringBuilder;

		// URL is optional. If present, must start with http. Assumed to be valid
		private string url = null;
		/// <summary>
		/// True if https in url, false if http
		/// </summary>
		public bool IsHTTPS = false;
		/// <summary>
		/// Full url in form : ["http"|"https" + "://?" +] querystring
		/// </summary>
		public string URL
		{
			get
			{
				if (url != null)
				{
					string qs = TheQueryString;
					stringBuilder.Length = 0;

					if (this.IsHTTPS)
						stringBuilder.Append("https://");
					else
						stringBuilder.Append("http://");

					stringBuilder.Append(url).Append("?").Append(qs);
					return stringBuilder.ToString();
				}
				else
					return TheQueryString;
			}
		}


		private QueryStringEncoding encoding;
		/// <summary>
		/// The encoding of the values for the QueryString
		/// </summary>
		public QueryStringEncoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				if (encoding == value)
					return;

				SetAllDirty();

				if (value == QueryStringEncoding.None)
				{
					Decode(encoding);
				}
				else
				{
					if (encoding != QueryStringEncoding.None)
					{
						Decode(encoding);
					}
					Encode(value);
				}

				encoding = value;
			}
		}

		/// <summary>
		/// internal string representation of the QueryString
		/// </summary>
		protected string queryString = null;
		// isDirty means out-of-sync with "this" NameValueCollection
		/// <summary>
		/// True if the string representation has not yet had changes propagated to it
		/// </summary>
		protected bool isDirtyQueryString = true;
		/// <summary>
		/// <!-- String representation in form: key1=value1[[&key2=value2]...[&keyn=valuen]] -->
		/// </summary>
		public string TheQueryString
		{
			get
			{
				bool first = true;
				if (stringBuilder == null || stringBuilder.Length > queryStringStorageHighWaterMark)
					stringBuilder = new StringBuilder(queryStringStorageHighWaterMark);
				else 
					stringBuilder.Length = 0;

				if (isDirtyQueryString)
				{
					// convert "this" NameValueCollection to string with optional URL
					foreach(string key in this.Keys)
					{
						if (! first)
							stringBuilder.Append("&");
						else
							first = false;
						stringBuilder.Append(key).Append("=").Append(this[key]);						
					}					
				}
				else
				{
					stringBuilder.Append(queryString);
				}

				queryString = stringBuilder.ToString();

				if (stringBuilder.Length > queryStringStorageHighWaterMark)
					queryStringStorageHighWaterMark = stringBuilder.Length;

				isDirtyQueryString = false;

				return stringBuilder.ToString();
			}
		}


		/// <summary>
		/// XML representation of the QueryString
		/// </summary>
		protected string xml = null;
		/// <summary>
		/// True if the XML representation has not yet had changes propagated to it
		/// </summary>
		protected bool isDirtyXML = true;
		/// <summary>
		/// String representation in XML format
		/// </summary>
		/// <remarks>The root element is named "QueryString" if there is no URL and it is the URL name if there is a URL. The elements within the root name have the same name as the key and the value is the value. Note that the URL is not part of the XML (could be changed)</remarks>
		public string XML
		{
			get
			{
				if (isDirtyXML)
				{
					if (stringBuilder == null || stringBuilder.Length > queryStringStorageHighWaterMark)
						stringBuilder = new StringBuilder(queryStringStorageHighWaterMark);
					else 
						stringBuilder.Length = 0;

					StringWriter sw = new StringWriter(stringBuilder);
					XmlTextWriter xtw = new XmlTextWriter(sw);
					if (url == null)
						xtw.WriteStartElement("QueryString");
					else
						xtw.WriteStartElement(url);

					// convert "this" NameValueCollection to string with optional URL
					foreach(string key in this.Keys)
					{
						xtw.WriteElementString(key, this[key]);					
					}
					
					xtw.WriteEndElement();

					isDirtyXML = false;
					return (xml = stringBuilder.ToString());
				}
				return xml;
			}

			set
			{
				xml = value;

				StringReader sr = new StringReader(xml);
				XmlTextReader xtr = new XmlTextReader(sr);

				xtr.MoveToContent();

				if (xtr.LocalName == "QueryString")
					url = null;
				else
				{
					url = xtr.LocalName;
				}

				this.Clear();

				while(xtr.Read())
				{
					while (xtr.NodeType == XmlNodeType.Element)
					{
						base.Add(xtr.LocalName, xtr.ReadElementString());
					}
				}

				SetAllDirty();
				isDirtyXML = false;
			}
		}

		/// <summary>
		/// XPathNavigator representation of the QueryString
		/// </summary>
		protected XPathNavigator xPathNavigator = null;
		/// <summary>
		/// True if the XPathNavigator representation has not yet had changes propagated to it
		/// </summary>
		protected bool isDirtyNavigator = true;
		/// <summary>
		/// An XPathNavigator representation of the querystring.
		/// </summary>
		/// <remarks>The URL is not included (could be changed though)</remarks>
		public XPathNavigator XPathNavigator
		{
			get
			{
				if (isDirtyNavigator || xPathNavigator == null)
				{
					StringReader sr = new StringReader(XML);
					XPathDocument xpd = new XPathDocument(sr);
						
					xPathNavigator = xpd.CreateNavigator();

					isDirtyNavigator = false;
				}
				return xPathNavigator;
			}
		}
		#endregion Data Members

		#region Constructor(s)
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <remarks>Use this overload if you want to initialize from XML input</remarks>
		public QueryString() {}
		/// <summary>
		/// Construct the QueryString object from an existing NameValueCollection
		/// </summary>
		/// <remarks>If there is a URL set the URL property independently and don't forget to set IsHTTP too.</remarks>
		/// <param name="nvc">the NameValueCollection</param>
		public QueryString(NameValueCollection nvc)
		{
			foreach(string key in nvc.Keys)
				base.Add(key, nvc[key]);
		}

		/// <summary>
		/// Construct the QueryString object from an existing NameValueCollection
		/// </summary>
		/// <param name="nvc">the NameValueCollection</param>
		/// <param name="encoding">The encoding, or lack of one, that already exists for the passed NameValueCollection</param>
		/// <remarks>If there is a URL set the URL property independently and don't 
		/// forget to set IsHTTP too. Encoding should be used to set the pre-existing encoding for the passed querystring. 
		/// Note that this is not the encoding to which you want the data converted but the encoding that the data is already in.</remarks>
		public QueryString(NameValueCollection nvc, QueryStringEncoding encoding)
		{
			this.encoding = encoding;

			foreach(string key in nvc.Keys)
				base.Add(key, nvc[key]);
		}

		/// <summary>
		/// Construct using string having key=value pair format with or without prefixing URL
		/// </summary>
		/// <param name="queryString">the querystring</param>
		public QueryString(string queryString)
		{
			this.queryString = queryString.Trim();
			this.IsHTTPS = false;
			this.url = QueryString.ParseUrl(this.queryString, ref this.IsHTTPS);

			PopulateCollection();
		}

		/// <summary>
		/// Construct using string having key=value pair format with or without prefixing URL
		/// </summary>
		/// <param name="queryString">the querystring</param>
		/// <param name="encoding">The encoding, or lack of one, that already exists for the passed NameValueCollection</param>
		/// <remarks>Encoding should be used to set the pre-existing encoding for the passed querystring. 
		/// Note that this is not the encoding to which you want the data converted but the encoding that the data is already in.</remarks>
		public QueryString(string queryString, QueryStringEncoding encoding)
		{
			this.queryString = queryString.Trim();
			this.IsHTTPS = false;
			this.url = QueryString.ParseUrl(this.queryString, ref this.IsHTTPS);

			PopulateCollection();

			this.encoding = encoding;
		}


		public QueryString AddRange(NameValueCollection nvc)
		{
			if (nvc != null)
			{
				foreach (string key in nvc.Keys)
					this.Add(key, nvc[key]);
			}
			return this;
		}

		private static string ParseUrl(string queryString, ref bool isHttps)
		{
			string url = null;

			int qsstartindex = 0;
			if (queryString.ToLower().IndexOf("http") == 0)
			{
				if ((qsstartindex = queryString.IndexOf("?")) > -1) 
				{
					if (queryString.ToLower().Substring(0, 5) == "https")
						isHttps = true;
					int index = queryString.IndexOf("://");
					if (index == -1)
						throw new LoggableException("QueryString class detected malformed URL: " + queryString, null);

					url = queryString.Substring(index + 3, qsstartindex - index - 3);
					queryString = queryString.Substring(qsstartindex + 1);
				}
				else
					throw new LoggableException("QueryString class detected malformed URL (missing \"?\") : " + queryString, null);
			}
			return url;
		}		
		#endregion Constructor(s)

		#region Overrides
		/// <summary>
		/// override indexer property so that other formats can be invalidated when this collection changes
		/// </summary>
		public new string this[string name] 
		{
			get
			{
				return base[name];
			}
			set
			{
				base[name] = value;
				SetAllDirty();
			}
		}
		/// <summary>
		/// override Add method so that other formats can be invalidated when this collection changes 
		/// </summary>
		/// <param name="key">key of item being added</param>
		/// <param name="value">value of item being added</param>
		public override void Add(string key, string value)
		{
			base.Add(key, value);
			SetAllDirty();
		}
		
		/// <summary>
		/// override Remove method so that other formats can be invalidated when this collection changes
		/// </summary>
		/// <param name="key">key of item to remove</param>
		public override void Remove(string key)
		{
			base.Remove(key);

			// then set everything Dirty
			SetAllDirty();
		}
		#endregion Overrides

	

		#region Helpers
		/// <summary>
		/// Set all representations (querystring, XML, and XPathNavigator "dirty" or "out-of-sync" with "this" NameValueCollection
		/// </summary>
		private void SetAllDirty()
		{
			isDirtyQueryString = true;
			isDirtyXML = true;
			isDirtyNavigator = true;
		}

		/// <summary>
		/// Transform this object using an XSLT from the specified file
		/// </summary>
		/// <param name="filename">XSLT filename</param>
		/// <returns>a StringWriter containing the results of the transformation</returns>
		public string Transform(string filename)
		{
			XslTransform xslt = new XslTransform();
			xslt.Load(filename);

			return Transform(xslt);
		}

		/// <summary>
		/// Transform the QueryString into a string using the specified XslTransform
		/// </summary>
		/// <param name="xslt">XslTransform used to transform the QueryString into the output string</param>
		/// <returns>the string result of the transformation</returns>
		public string Transform(XslTransform xslt)
		{
			//TODO check need to reset sb length
			if (stringBuilder == null || stringBuilder.Length > queryStringStorageHighWaterMark)
				stringBuilder = new StringBuilder(queryStringStorageHighWaterMark);
			stringBuilder.Length = 0;
			StringWriter sw = new StringWriter(stringBuilder);
			XPathNavigator xpn = this.XPathNavigator;
			stringBuilder.Length = 0;
			xslt.Transform(xpn, null, sw, null);
			if (stringBuilder.Length > queryStringStorageHighWaterMark)
				queryStringStorageHighWaterMark = stringBuilder.Length;
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Return a StringBuilder based on the current hwm.
		/// </summary>
		/// <param name="length">the desired length used to initialize the string builder</param>
		/// <returns>a StringBuilder initialized to a length that is the greater of input length or hwm</returns>
		protected StringBuilder GetSufficientSizedStringBuilder(int length)
		{
			if (length > queryStringStorageHighWaterMark)
			{
				stringBuilder = new StringBuilder(queryStringStorageHighWaterMark);
				queryStringStorageHighWaterMark = length;
			}
			else
				stringBuilder.Length = 0;

			return stringBuilder;
		}

		private void PopulateCollection()
		{
			// did we have ?& situation?
			if (this.queryString.Substring(0, 1) == "&")
				throw new LoggableException("QueryString class detected malformed QueryString: " + this.queryString, null);

			char[] seps = {'&'};
			string[] pairs = this.queryString.Split(seps);

			// check for && (double amp)
			foreach (string pair in pairs)
			{
				if (pair == null || pair.Length == 0)
					throw new LoggableException("QueryString class detected malformed QueryString: " + this.queryString, null);
			}

			StringCollection dupchecker = new StringCollection();
			
			foreach(string pair in pairs)
			{
				int index2;		
				// each param must have "="
				if ((index2 = pair.IndexOf("=")) == -1)
					throw new LoggableException("QueryString class detected malformed QueryString: " + this.queryString, null);

				string val = null;
				// set val only if it exists (to avoid exception using Substring)
				if (index2 + 1 != pair.Length) 
					val = pair.Substring(index2 + 1, pair.Length - index2 - 1);
				string key = pair.Substring(0, index2);

				// check for duplicate keys
				if (dupchecker.Contains(key))
					throw new LoggableException("QueryString class detected malformed QueryString (duplicate key= " + key + " : " + this.queryString, null);
				dupchecker.Add(key);

				this.Add(key, val);
			}
		}


	
		private void Encode(QueryStringEncoding encoding)
		{
			string newval = null;

			for(int i = 0; i < this.Keys.Count; i++)
			{
				if (encoding == QueryStringEncoding.Html)
					newval = HttpUtility.HtmlEncode(this[Keys[i]]);
				else
					newval = HttpUtility.UrlEncode(this[Keys[i]]);

				this[Keys[i]] = newval;
			}
		}

		private void Decode(QueryStringEncoding encoding)
		{
			for(int i = 0; i < this.Keys.Count; i++)
			{
				string newval = null;

				if (encoding == QueryStringEncoding.Html)
					newval = HttpUtility.HtmlDecode(this[Keys[i]]);
				else
					newval = HttpUtility.UrlDecode(this[Keys[i]]);

				this[Keys[i]] = newval;
			}
		}



		#region File-related
		/// <summary>
		/// Reads a file containing a single querystring in name-value-pair format
		/// </summary>
		/// <param name="filename">full path of file containing querystring</param>
		/// <param name="encoding">encoding of the file being read</param>
		/// <returns></returns>
		public static QueryString InitFromNVPairFile(string filename, System.Text.Encoding encoding)
		{
			FileStream fs = null;
			StreamReader sr = null;
			string data = null;
			QueryString qs = null;

			try
			{
				fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
				sr = new StreamReader(filename, encoding);
				data = sr.ReadToEnd();
				qs = new QueryString(data);
			}
			catch(Exception ex)
			{
			}
			finally
			{
				if (sr != null) 
					sr.Close();
				qs = null;
			}

			return qs;
		}

		/// <summary>
		/// Reads a file containing a single querystring in XML format
		/// </summary>
		/// <param name="filename">full path of the file being containing the querystring</param>
		/// <param name="encoding">encoding of the file being read</param>
		/// <returns></returns>
		public static QueryString InitFromXmlFile(string filename, Encoding encoding)
		{
			StreamReader sr = null;
			QueryString qs = new QueryString();

			try
			{
				sr = new StreamReader(filename, encoding);
				qs.XML = sr.ReadToEnd();
			}
			catch(Exception ex)
			{
				qs = null;
				//new LoggableException(ex);
			}
			finally
			{
				if (sr != null)
					sr.Close();
			}

			return qs;
		}
		#endregion File-related

		#endregion Helpers
	}

}
