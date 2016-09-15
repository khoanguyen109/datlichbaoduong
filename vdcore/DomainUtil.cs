using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Vendare.Utils
{
	public class DomainUtil
	{
		// singleton object
		private static DomainUtil instance = null;
        private List<string> _TLDList = null;
        public List<string> TLDList
        {
            get { return _TLDList;}
        }

		// singleton method
		public static DomainUtil Instance()
		{
			lock(typeof(DomainUtil)) 
			{
				if(instance == null)
                {
					instance = new DomainUtil();
                }

				return instance;
			}
		}

		private DomainUtil()
		{
            _TLDList = XmlParser.GetNodeValues(@"Vendare.DATA.TLD.xml", @"/TLDList/name", null);
        }

        //1. requires at least one subdomain
        //2. allows shortest top-level domains like "ca" and "museum" as longest.
        //3. Labels/parts should be seperated by period.
        //4. Each label/part has maximum of 63 characters.
        //5. First and last character of label must be alphanumeric, other characters alphanumeric or hyphen.
        //6. Max length of domain is 253 characters of text.
        public bool ValidDomain(string domainName)
        {
            bool result = false;

            if (null != domainName && domainName.Length < 254)
            {
                Regex regEx = new Regex("^([a-zA-Z0-9]([a-zA-Z0-9\\-]{0,61}[a-zA-Z0-9])?\\.)+[a-zA-Z]{2,6}$");
                Match m1 = regEx.Match(domainName);

                if (m1.Success)
                {
                    result = true;
                }
            }

            return result;
        }

        // return everything before the query string
        public string RemoveQ(string aString)
        {
            string[] strArr = { };

            if (null != aString)
            {
                Regex regEx = new Regex("\\?");
                Match m1 = regEx.Match(aString);

                if (m1.Success)
                {
                    strArr = regEx.Split(aString);
                }

                if (strArr.Length > 0)
                {
                    aString = strArr[0];
                }
            }

            return aString;
        }

		public string ReturnQuerystring(string url)
		{
			if (!url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) &&
				!url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
				url = url + "http://";
			Uri tempUri = new Uri(url);
			return tempUri.Query;
		}

		public string RemoveSubdomainAndQueryStr(Uri url)
        {
            string result = null;

            string domain = "";
            GetHost(url.Host, out domain);
            result = url.Scheme + "://" + domain;

            int port = url.Port;
            if (port > 0 && port != 80)
            {
                result = result + ":" + url.Port.ToString();
            }

            result = result + RemoveQ(url.PathAndQuery);

            return result;
        }

        /// <summary>
        /// Takes the Full Domain Name and tries to remove sub domains, by vaidating across Primary/Secondary TLD List
        /// Final domain can be accessed from out param while return parameter states whether its a valid domain or not as per TLD norms
        /// Removes beginning "www." and trailing "." characters
        /// Returns a valid IP addr AS-IS.
        /// Strips off anything preceedng domain and tld.
        /// </summary>
        /// <param name="host"></param>
        /// <returns> Returns domain and tld.</returns>
        public bool GetHost(string host, out string domain)
        {
            bool res = true;
            domain = "";

            if (host == null)   // NULL OBJECT
                return false;
            
            if (host.EndsWith("."))
                host = host.Substring(0, host.Length - 1);
            else if(host.ToLower().StartsWith(@"www."))
                host = host.Substring(4, host.Length - 4);

            // if host is a valid ip, return as is
            if (Helper.ValidIPAddress(host))
            {
                domain = host;
                res = false;
            }
            else
            {
                
                string[] hostParts = host.Split(new char[] { '.' });
                int idx = hostParts.Length;
                string ext = "";
                switch (idx)
                { 
                    case 1:     // NO TLD
                        res = false;
                        domain = host;
                        break;
                    case 2:     // PRIMARY TLD
                        ext = hostParts[idx - 1].ToLower();
                        domain = host;
                        if (!_TLDList.Contains(ext))
                            res = false;
                        break;
                    case 3:     // SECONDARY TLD OR SUB-DOMAIN
                        ext = hostParts[idx - 2].ToLower() + "." + hostParts[idx - 1].ToLower(); // SECOND LEVEL
                        if (_TLDList.Contains(ext))
                            domain = host;
                        else
                        {
                            domain = hostParts[idx - 2] + "." + hostParts[idx - 1];             // PRIMARY LEVEL
                            if (!_TLDList.Contains(hostParts[idx - 1].ToLower()))
                                res = false;
                        }
                        break;
                    default:
                        ext = hostParts[idx - 3].ToLower() + "." + hostParts[idx - 2].ToLower() + "." + hostParts[idx - 1].ToLower();
                        if (_TLDList.Contains(ext))
                            domain = hostParts[idx - 4].ToLower() + "." + hostParts[idx - 3].ToLower() + "." + hostParts[idx - 2].ToLower() + "." + hostParts[idx - 1].ToLower();
                        else
                        {
                            ext = hostParts[idx - 2].ToLower() + "." + hostParts[idx - 1].ToLower();
                            if (_TLDList.Contains(ext))
                                domain = hostParts[idx - 3].ToLower() + "." + hostParts[idx - 2].ToLower() + "." + hostParts[idx - 1].ToLower();
                            else
                            {
                                domain = hostParts[idx - 2] + "." + hostParts[idx - 1];
                                if (!_TLDList.Contains(hostParts[idx - 1].ToLower()))
                                    res = false;
                            }
                        }
                        break;
                }
            }
            return res;
        }

        public bool GetHostWithoutTLD(string host, out string name)
        {
            var domain = string.Empty;
            name = string.Empty;

            if (!GetHost(host, out domain))
                return false;

            string[] domainParts = domain.Split(new char[] { '.' });
            if (domainParts.Length < 2)
                return false;
            else
                name = domainParts[0];

            var tld = string.Join(".", domainParts.Skip(1).ToArray());
            if (_TLDList.Contains(tld))
                return true;

            return false;
        }
	}
}

