using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
/// <summary>
/// utilities for xml and regex
/// </summary>
/// 
namespace Vendare.Utils
{
	public class StringUtil
	{
		private const String COMMA = ",";
		private const String QUOTE = "\"";
		private const String EMPTY_STRING = "";

		private const String LESS_THAN = "<";
		private const String LESS_THAN_ESCAPED = "&lt;";
		private const String GREATER_THAN = ">";
		private const String GREATER_THAN_ESCAPED = "&gt;";
		private const String AMPERSAND = "&";
		private const String AMPERSAND_ESCAPED = "&amp;";
		private const String APOS = "'";
		private const String APOS_ESCAPED = "&apos;";

		// singleton obj
		private static StringUtil instance = null;
		
		// singleton method
		public static StringUtil GetInstance()
		{
			lock(typeof(StringUtil)) 
			{
				if(instance == null)
					instance = new StringUtil();

				return instance;
			}
		}

		private StringUtil()
		{
		}

		// parses out subdomains and extensions
		public String GetDomain(String url)
		{
			return GetDomain(url, false);
		}

		// parses out subdomains
		public String GetDomain(String url, bool returnExtension)
		{			
			int lastDot = url.LastIndexOf(".");
			if(lastDot < 0)
				return url;

			int secondToLastDot = url.Substring(0, lastDot).LastIndexOf(".");

			if(secondToLastDot < 0)
			{
				secondToLastDot = 0;
			}

			if (returnExtension)
				return url.Substring(secondToLastDot, url.Length - secondToLastDot);
			else
                return url.Substring(secondToLastDot, lastDot - secondToLastDot);
		}

       
        // splits a string to a list based on the pattern (\W) skipping blank elements returned by regex
		public ArrayList SplitToArrayList(String lineOfText, String pattern, bool skipBlank)
		{
			pattern = EscapeRegexChars(pattern);
			ArrayList elements = new ArrayList();
			foreach(string element in Regex.Split(lineOfText, pattern))
			{
				if((skipBlank) && (element.Trim().Length == 0))	{}
				else
				{
					elements.Add(element.Trim());
				}
			}
			return elements;
		}


		// splits a string to a list based on the pattern (\W)
		public ArrayList SplitToArrayList(String lineOfText, String pattern)
		{
			return SplitToArrayList(lineOfText, pattern, false);
		}

		// turns a list into a string delimited by delim
		public String ArrayListToString(ArrayList list, String delim)
		{
			if(list == null)
				return null;

			StringBuilder text = null;
			String ele = null;
			foreach(Object element in list)
			{
				ele = Convert.ToString(element);

				if(text == null)
					text = new StringBuilder(ele);

				else if((ele != null) && (ele.Length > 0))
					text.Append(delim).Append(ele);

			}
			if(text == null)
				return null;
			else
				return text.ToString();
		}

		// turns a list into a string delimited by delim
		public String HashtableToString(Hashtable hash, String delim)
		{
			if(hash == null)
				return null;

			StringBuilder text = new StringBuilder();
			IDictionaryEnumerator myEnumerator = hash.GetEnumerator();

			while ( myEnumerator.MoveNext() )
			{
				if(myEnumerator.Value is Hashtable)
					text.Append(myEnumerator.Key+"="+HashtableToString((Hashtable)myEnumerator.Value, ",")).Append(delim);				
				else
					text.Append(myEnumerator.Key+"="+myEnumerator.Value).Append(delim);
			}

			return text.ToString();
		}
/*
		public ArrayList StringToHashtables(String text, char hashDelim, char bucketDelim)
		{
			if(text == null)
				return null;

			ArrayList list = new ArrayList(3);

			if(text.IndexOf(hashDelim) == -1)
			{
				Hashtable temp = StringToHashtable(hash, bucketDelim);
				if(temp != null)
					list.Add(temp);

				return list;
			}

			String[] hashes = text.Split(hashDelim);
			foreach(String hash in hashes)
			{
				Hashtable temp = StringToHashtable(hash, bucketDelim);
				if(temp != null)
					list.Add(temp);
			}

			return list;
		}

		// turns text into a string delimited by delim
		public Hashtable StringToHashtable(String text, char delimiter)
		{
			if(text == null)
				return null;

			if(text.IndexOf(delimiter) == -1)
				return null;

			Hashtable hash = new Hashtable(3);
			String[] buckets = text.Split(delimiter);
			foreach(String bucket in buckets)
			{
				if(bucket.IndexOf("=") == -1)
					continue;

				String[] pair = bucket.Split("=");
				hash[pair[0]] = pair[1];
			}

			return hash;
		}
*/
		// extracts part of a string using regex groups
		public String GetGroupToString(String pattern, String name, String input) 
		{
			Regex regex = new Regex(pattern);
			Match match = regex.Match(input);
			Group group = match.Groups[name];
			if(group != null)
				return group.ToString();
			else
				return null;
		}

		// extracts part of a string using regex groups
		public Hashtable GetGroupToHash(Regex regex, String input) 
		{
			if((regex == null) || (input == null))
				return null;

			Match match = regex.Match(input);

			if(!match.Success)
				return null;

			ArrayList groups = new ArrayList(regex.GetGroupNames());

			Hashtable hash = new Hashtable(groups.Count);
			String temp = null;
			foreach(String group in groups)
			{
				temp = match.Groups[group].ToString();
				if((temp == null) || temp.Equals(""))
					continue;

				// if the group is a numeric then ignore and continue
				try	{ Convert.ToInt32(group); continue;	} 
				catch(Exception)	{}

				hash[group] = temp;
			}

			return hash;
		}

		// used to escape regex special chars
		public String EscapeRegexChars(String pattern)
		{
			if(pattern == null)
				return null;

			pattern = pattern.Replace("*", "\\*");
			pattern = pattern.Replace("+", "\\+");
			pattern = pattern.Replace("@", "\\@");
			pattern = pattern.Replace("#", "\\#");
			pattern = pattern.Replace("^", "\\^");
			pattern = pattern.Replace("[", "\\[");
			pattern = pattern.Replace("]", "\\]");
			pattern = pattern.Replace("$", "\\$");
			pattern = pattern.Replace("<", "\\<");
			pattern = pattern.Replace(">", "\\>");
			pattern = pattern.Replace("(", "\\(");
			pattern = pattern.Replace(")", "\\)");
			pattern = pattern.Replace("|", "\\|");
			return pattern;
		}

		// escapes quotes and stuff for database insertion.
		public String EscapeDatabaseSensitiveChars(String data)
		{
			data = data.Replace(QUOTE, EMPTY_STRING);
			data = data.Replace(LESS_THAN, LESS_THAN_ESCAPED);
			data = data.Replace(GREATER_THAN, GREATER_THAN_ESCAPED);
			data = data.Replace(AMPERSAND, AMPERSAND_ESCAPED);
			data = data.Replace(APOS, APOS_ESCAPED);
			data = data.Trim();
			return data;
		}

		public String GetExceptionMessage(Exception e)
		{
			String msg = "";
			GetExceptionMessage(e, ref msg);
			return msg;
		}

		private void GetExceptionMessage(Exception e, ref String msg) 
		{		
			if(msg == null)
				msg = "";

			msg += e.ToString();

			if(e.InnerException != null)
				GetExceptionMessage(e.InnerException, ref msg);
		}

		public void StringToFile(String text, String file)
		{
			StreamWriter writer = null;
			try
			{
				FileInfo info = new FileInfo(file);
				writer = info.CreateText();
				writer.Write(text);
			}
			finally
			{
				if(writer != null)
					writer.Close();
			}
		}


		/// <summary>
		/// URLAppendParam
		/// Appends key=val to landingPage.
		/// if the landing page ends with a "/", it appends a ?key=val
		/// if the landing page ends with a "?", it appends a key=val
		/// if the landing page contains a "?", it appends a &key=val
		/// all other cases, just appends a ?key=val
		/// </summary>
		/// <param name="landingPage">landing page to append parameters to</param>
		/// <param name="key">parameter key</param>
		/// <param name="val">parameter value</param>
		/// <returns></returns>
		public String URLAppendParam( String landingPage, String key, String val )
		{
			if((key == null) || (val == null) || key.Equals(""))
				return landingPage;

			if(landingPage.EndsWith("/"))
			{
				//landingPage = landingPage.Substring(0, landingPage.Length -1);
				landingPage += "?"+key+"="+val;
			}
			else if(landingPage.EndsWith("?"))
			{
				landingPage += key+"="+val;
			}
			else if(landingPage.IndexOf("?") != -1)
			{
				landingPage += "&"+key+"="+val;
			}
			else
			{
				landingPage += "?"+key+"="+val;
			}

			return landingPage;

		}


		public String URLAppendParam( String landingPage, String param )
		{
			if(landingPage.EndsWith("/"))
			{
				//landingPage = landingPage.Substring(0, landingPage.Length -1);
				landingPage += "?"+param;
			}
			else if(landingPage.EndsWith("?"))
			{
				landingPage += param;
			}
			else if(landingPage.IndexOf("?") != -1)
			{
				landingPage += "&"+param;
			}
			else
			{
				landingPage += "?"+param;
			}

			return landingPage;

		}
        /*
         * Returns whether searchString is an element of searchArray
         * Requires:    searchArray is not null
         * Ensures:     If searchString matches the value of at least one
         *                  element of searchArray, return true
         *                      -searchString can be null in which case if any
         *                          of the elements of the array are null, it
         *                          will 'match'
         *              Else, return false
         *      Last updated: Steven Maus, 3/1/2007
         */
        public bool ContainsInStringArray(string[] searchArray, string searchString)
        {
            bool contained = false;
            int index = 0;
            int size = searchArray.Length;
            while ((contained == false) && (index < size))
            {
                contained = ((String)searchArray[index]).Trim().Equals(searchString);
                index = index + 1;
            }
            return contained;
        }

		/// <summary>
		/// Convert a string to a Stream
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public Stream StringToStream(string str)
		{
			if (str == null) str = "";
			byte[] byteArray = Encoding.ASCII.GetBytes(str);
			MemoryStream stream = new MemoryStream(byteArray);
			return stream;
		}

		/// <summary>
		/// Pass in a url string and get back a script tag with window.location to the url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public string WindowLocationRedirect(string url)
		{
			if (url == null || url == "") 
				return "";
			else 
				return "<script type=\"text/javascript\">window.location = \"" + url + "\"</script>";
		}

		/// <summary>
		/// Pass in a string and get back a document.write statement for the string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string DocumentWrite(string str)
		{
			if (str == null || str == "")
				return "";
			else
				return "document.write('" + str + "');";
 
		}

		public String ConstructQueryString(NameValueCollection parameters)
		{
			List<String> items = new List<String>();
			//foreach (String name in parameters)
			//    items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

			for (int i = 0; i < parameters.Count; i++)
			{
				items.Add(String.Concat(parameters.GetKey(i), "=", System.Web.HttpUtility.UrlEncode(parameters.Get(i))));
			}
			return String.Join("&", items.ToArray());
		}

        public string TruncateFromComma(string text)
        {
            if (text != null)
            {
                if (text.IndexOf(",") > 0)
                    text = text.Substring(0, text.IndexOf(","));
            }
            return text;
        }

	}

	public static class StringExtensions
	{
		public static string RemoveAfterComma(this String text)
		{
			text = StringUtil.GetInstance().TruncateFromComma(text);
			return text;
		}
	}
}
