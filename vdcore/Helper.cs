using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Vendare.DBAccess;
using Vendare.Error;
using System.Threading;

namespace Vendare.Utils
{
    /// <summary>
    /// The Helper class contains static utility methods.
    /// </summary>
    public class Helper
    {
        private static String[,] withApos = { { "&", "&amp;" }, { "<", "&lt;" }, { ">", "&gt;" }, { "\"", "&quot;" }, { "\'", "&apos;" } };
        private static String[,] noApos = { { "&", "&amp;" }, { "<", "&lt;" }, { ">", "&gt;" }, { "\"", "&quot;" } };
        private static ArrayList _countries = null;
        private static Hashtable _statesAndProvinces = null;
        private static ArrayList _badWords = null;
        private static ArrayList _veryBadWords = null;
        private static string commonDbStr = "common.dbo.";

        static Helper()
        {
            if (ConfigurationManager.AppSettings.Get("commonDB") != null && ConfigurationManager.AppSettings.Get("commonDB").Length > 0)
                commonDbStr = ConfigurationManager.AppSettings.Get("commonDB").ToString() + ".dbo.";

            ThreadPool.QueueUserWorkItem(LoadData);
        }

        #region Internal static methods

        internal static object veryBadWordsLock = new object();
        internal static object badWordsLock = new object();
        internal static object statesLock = new object();
        internal static object countriesLock = new object();

        internal static ArrayList VeryBadWords
        {
            get
            {
                ArrayList vdw = null;
                lock (veryBadWordsLock)
                {
                    vdw = _veryBadWords ?? _veryBadWords.Clone() as ArrayList;
                }
                return vdw;
            }
            set
            {
                lock (veryBadWordsLock)
                {
                    _veryBadWords = value;
                }
            }
        }

        internal static ArrayList BadWords
        {
            get
            {
                ArrayList vdw = null;
                lock (badWordsLock)
                {
                    vdw = _badWords ?? _badWords.Clone() as ArrayList;
                }
                return vdw;
            }
            set
            {
                lock (badWordsLock)
                {
                    _badWords = value;
                }
            }
        }

        internal static ArrayList Countries
        {
            get
            {
                ArrayList vdw = null;
                lock (countriesLock)
                {
                    vdw = _countries ?? _countries.Clone() as ArrayList;
                }
                return vdw;
            }
            set
            {
                lock (countriesLock)
                {
                    _countries = value;
                }
            }
        }

        internal static Hashtable StatesAndProvinces
        {
            get
            {
                Hashtable vdw = null;
                lock (statesLock)
                {
                    vdw = _statesAndProvinces ?? _statesAndProvinces.Clone() as Hashtable;
                }
                return vdw;
            }
            set
            {
                lock (statesLock)
                {
                    _statesAndProvinces = value;
                }
            }
        }

        private static void LoadData(object state)
        {
            DataConnection conn = null;
            try
            {
                conn = new DataConnection();
                LoadCountries(conn);
                LoadStatesAndProvinces(conn);
                LoadVeryBadwords(conn);
                LoadBadWords(conn);
            }
            catch (Exception e)
            {
                new LoggableException(e, null);
            }
            finally
            {
                if (conn != null)
                    conn.CloseConnection();
            }

        }

        private static void LoadBadWords(DataConnection conn)
        {
            var badWords = new ArrayList();
            var badWordReader = conn.Execute("select * from " + commonDbStr + "user_profanity_filter where filter_level = 0");
            while (badWordReader.Read())
                badWords.Add(badWordReader.GetString(0));
            badWordReader.Close();
            BadWords = badWords;
        }

        private static void LoadVeryBadwords(DataConnection conn)
        {
            var veryBadWords = new ArrayList();
            var veryBadWordReader = conn.Execute("select * from " + commonDbStr + "user_profanity_filter where filter_level = 1");
            while (veryBadWordReader.Read())
                veryBadWords.Add(veryBadWordReader.GetString(0));

            veryBadWordReader.Close();
            VeryBadWords = veryBadWords;
        }

        private static void LoadStatesAndProvinces(DataConnection conn)
        {
            var statesAndProvinces = new Hashtable();
            var stateReader = conn.Execute("select country_id, state_abrev from " + commonDbStr + "states (nolock) union select country_id, upper(state) from " + commonDbStr + "states (nolock)");
            while (stateReader.Read())
            {
                if (statesAndProvinces[stateReader.GetString(0)] == null)
                {
                    ArrayList temp = new ArrayList();
                    temp.Add(stateReader.GetString(1));
                    statesAndProvinces.Add(stateReader.GetString(0), temp);
                }
                else
                {
                    ArrayList temp = (ArrayList)statesAndProvinces[stateReader.GetString(0)];
                    temp.Add(stateReader.GetString(1));
                }
            }
            stateReader.Close();
            StatesAndProvinces = statesAndProvinces;
        }

        private static void LoadCountries(DataConnection conn)
        {
            var countries = new ArrayList();
            var countryReader = conn.Execute("select country_id from " + commonDbStr + "countries (nolock)");
            while (countryReader.Read())
                countries.Add(countryReader.GetString(0));

            countries.Add("uk");
            countryReader.Close();
            Countries = countries;
        }

        #endregion

        /// <summary>
        /// Converts a string into an XML legal string by conducting entity substitutions
        /// </summary>
        /// <param name="text">Original string to convert</param>
        /// <returns>XML Legal String</returns>
        public static String SafeXMLString(String text)
        {
            return SafeXMLString(text, false);
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
        public static String SafeXMLString(String text, bool ignoreApos)
        {
            if (IsEmpty(text))
                return text;

            String[,] entities;

            if (!ignoreApos)
                entities = withApos;
            else
                entities = noApos;

            for (int i = 0; i < entities.Length / 2; i++)
            {
                text = text.Replace(entities[i, 0], entities[i, 1]);
            }

            return text;
        }

        /// <summary>
        ///converts a string to a bool based on the assumption that 
        ///if the string = 'n' or is empty then it is false, otherwise it is true
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <returns>Converted bool value</returns>
        public static bool ConvertToBool(String value)
        {
            if (IsEmpty(value))
                return false;

            if (value.ToLower().Equals("n"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// converts a bool to a string based on the assumption that 
        /// if the value is false then the string = 'n', otherwise it is "y"
        /// </summary>
        /// <param name="value">bool to convert</param>
        /// <returns>Converted String value</returns>
        public static String ConvertToString(bool value)
        {
            if (value)
                return "y";
            else
                return "n";
        }

        /// <summary>
        /// strips string of all non numeric characters
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>Converted ordinal value</returns>
        public static String ConvertToOrdinal(String value)
        {
            if (IsEmpty(value))
                return "";

            char[] charPhone = value.ToCharArray();
            int length = charPhone.Length;

            char c;
            for (int i = 0; i < length; i++)
            {
                c = charPhone[i];
                if (c < '0' || c > '9')
                {
                    for (int ii = i; ii < length - 1; ii++)
                        charPhone[ii] = charPhone[ii + 1];
                    --length;
                }
            }

            return new String(charPhone, 0, length);
        }

        /// <summary>
        /// tests a string to make sure it is not null or empty
        /// </summary>
        /// <param name="value">String to test</param>
        /// <returns>True if the sting is null or is empty or only contains empty spaces</returns>
        public static bool IsEmpty(String value)
        {
            bool test = false;

            if (value == null)
                test = true;
            else if (value.Trim().Equals(""))
                test = true;

            return test;
        }

        /// <summary>
        /// truncates a string if over a specified length
        /// </summary>
        /// <param name="value">String to truncate</param>
        /// <param name="length">Length limit of string</param>
        /// <returns>truncated string</returns>
        public static String truncate(String value, int length)
        {
            if (value == null)
                return null;

            if (value.Length > length)
                value = value.Substring(0, length);

            return value.Trim();
        }

        /// <summary>
        /// Capitalizes the first letter of each word
        /// </summary>
        /// <param name="text">text to convert</param>
        /// <returns>a string where the first letter of every word is capitalized</returns>
        public static String SmartCase(String text)
        {
            StringBuilder smartStr = new StringBuilder(100);

            if (IsEmpty(text))
                return text;

            int pos = 0;
            int target = 0;
            text = text.Trim();

            while (pos < text.Length)
            {
                smartStr.Append(text.Substring(pos, 1).ToUpper());
                if (++pos < text.Length)
                {
                    target = text.IndexOf(" ", pos);
                    if (target > pos)
                    {
                        smartStr.Append(text.Substring(pos, target - pos).ToLower());
                        smartStr.Append(" ");
                        pos = target + 1;
                    }
                    else
                    {
                        target = text.Length;
                        smartStr.Append(text.Substring(pos, target - pos).ToLower());
                        pos = target;
                    }
                }
            }

            return smartStr.ToString();
        }

        public static bool ValidCountry(String country)
        {
            if ("|us|ut|ca|".IndexOf("|" + country.ToLower() + "|") > -1)
                return true;

            return Countries.Contains(country.ToLower());
        }

        public static bool ValidUSOrCanStateOrProv(String country, String stateOrProvince)
        {
            ArrayList states = (ArrayList)StatesAndProvinces[country];
            if (states == null)
                return false;

            bool test = states.Contains(stateOrProvince.ToUpper());
            return test;
        }

        public static bool IsNotBadWord(String text)
        {
            if (text == null)
                return true;

            foreach (String word in VeryBadWords)
            {
                if (text.IndexOf(word) != -1)
                    return false;
            }

            ArrayList list = new ArrayList(Regex.Split(text, @"\W"));
            foreach (String word in BadWords)
            {
                if (text.StartsWith(word))
                    return false;
                else if (text.EndsWith(word))
                    return false;

                if (list.Contains(word))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verifies that a Proper Name is valid
        /// </summary>
        /// <param name="text">Name to verify</param>
        /// <returns>True if the format is valid</returns>
        public static bool ValidName(String text)
        {
            if (IsEmpty(text))
                return false;

            bool success = false;

            Regex re = new Regex("([^a-z\\.\\s\\-\\'\\,]+)");
            Match m = re.Match(text.ToLower());
            success = !m.Success;

            if (!success)
                return success;
            else
                return IsNotBadWord(text);
        }

        /// <summary>
        /// Verifies that an address line is correctly formated
        /// </summary>
        /// <param name="text">adress line to format</param>
        /// <returns>True if the format is valid</returns>
        public static bool ValidAddress(String text)
        {
            if (IsEmpty(text))
                return false;

            bool success = false;

            Regex re = new Regex("([^a-z0-9\\.\\s\\-\\'\\,\\#\\/]+)");
            Match m = re.Match(text.ToLower());

            success = !m.Success;

            if (!success)
                return success;
            else
                return IsNotBadWord(text);
        }

        /// <summary>
        /// Verifies an Ip Address is correctly formatted
        /// </summary>
        /// <param name="text">Ip Address to test</param>
        /// <returns>True if the format is valid</returns>
        public static bool ValidIPAddress(String text)
        {
            if (IsEmpty(text))
                return false;
            Regex re = new Regex(@"^((0|1[0-9]{0,2}|2[0-9]{0,1}|2[0-4][0-9]|25[0-5]|[3-9][0-9]{0,1})\.){3}(0|1[0-9]{0,2}|2[0-9]{0,1}|2[0-4][0-9]|25[0-5]|[3-9][0-9]{0,1})$");
            return re.IsMatch(text);
        }

        /// <summary>
        /// Verifies an email address is correctly formatted
        /// </summary>
        /// <param name="text">email to test</param>
        /// <returns>True if the format is valid</returns>
        public static bool ValidEmail(String text)
        {
            if (IsEmpty(text))
                return false;
            Regex re = new Regex(@"^[A-Za-z0-9][\w\.\-]+@[A-Za-z0-9.-]+\.[A-Z_a-z]{2,}$");
            Match m = re.Match(text.ToLower());
            if (m.Success)
            {
                int dotPos = text.LastIndexOf(".");
                String exten = text.Substring(dotPos + 1, text.Length - dotPos - 1);
                exten = exten.ToLower();
                if (!("|com|info|net|tv|ws|edu|org|gov|mil|us|ca|cc|cn|biz|".IndexOf("|" + exten + "|") > -1))
                    return ValidCountry(exten);
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Verifies an email address is correctly formatted with valid tlds
        /// </summary>
        /// <param name="text">email to test</param>
        /// <returns>True if the format is valid</returns>
        public static bool ValidEmailTlds(String text)
        {
            if (IsEmpty(text))
                return false;
            Regex re = new Regex(@"^[A-Za-z0-9][\w\.\-]+@[A-Za-z0-9.-]+\.[A-Z_a-z]{2,}$");
            Match m = re.Match(text.ToLower());
            if (m.Success)
            {
                int dotPos = text.LastIndexOf(".");
                String exten = text.Substring(dotPos + 1, text.Length - dotPos - 1);
                exten = exten.ToLower();
                if (!("|com|info|net|tv|ws|edu|org|gov|mil|us|ca|cc|cn|biz|".IndexOf("|" + exten + "|") > -1))
                    return DomainUtil.Instance().TLDList.Contains(exten);
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Verifies if the email domain is valid. It will accept any number of "*" before the domain. eg: **@jackpot.com
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ValidEmailDomain(String text)
        {
            if (IsEmpty(text))
                return false;
            Regex re = new Regex(@"^[\*]+@[A-Za-z0-9.-]+\.[A-Z_a-z]{2,}$");
            Match m = re.Match(text.ToLower());
            if (m.Success)
            {
                int dotPos = text.LastIndexOf(".");
                String exten = text.Substring(dotPos + 1, text.Length - dotPos - 1);
                exten = exten.ToLower();
                if (!("|com|info|net|tv|ws|edu|org|gov|mil|us|ca|cc|cn|biz|".IndexOf("|" + exten + "|") > -1))
                    return ValidCountry(exten);
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Verifies if an area code/prefix/suffix array resolves to a valid phone number
        /// </summary>
        /// <param name="phone">String array holding an area code/prefix/suffix</param>
        /// <returns>An array of booleans indicating which elements passed</returns>
        public static bool[] ValidPhone(String[] phone)
        {
            bool[] result = { true, true, true };
            int areaCode = 0;
            int prefix = 0;
            int suffix = 0;

            try
            {
                areaCode = Int32.Parse(phone[0]);
            }
            catch (Exception)
            {
                result[0] = false;
            }

            try
            {
                prefix = Int32.Parse(phone[1]);
            }
            catch (Exception)
            {
                result[1] = false;
            }

            try
            {
                suffix = Int32.Parse(phone[2]);
            }
            catch (Exception)
            {
                result[2] = false;
            }

            if (areaCode < 201 || areaCode > 999 || areaCode == 800 || areaCode == 877 || areaCode == 900 || areaCode == 976)
                result[0] = false;

            if (prefix < 200 || prefix > 999 || prefix == 555 || prefix == 411 || prefix == 611)
                result[1] = false;

            if ((phone == null || phone.Length < 3 || phone[2] == null) || (result[1] && phone[2].Length != 4))
                result[2] = false;

            return result;
        }

        public static bool[] ValidPhone(String phone)
        {
            bool[] result = { true, true, true };
            int areaCode = 0;
            int prefix = 0;
            int suffix = 0;
            String ordPhone = Helper.ConvertToOrdinal(phone);

            try
            {
                areaCode = Int32.Parse(ordPhone.Substring(0, 3));
            }
            catch (Exception)
            {
                result[0] = false;
            }

            try
            {
                prefix = Int32.Parse(ordPhone.Substring(3, 3));
            }
            catch (Exception)
            {
                result[1] = false;
            }

            try
            {
                suffix = Int32.Parse(ordPhone.Substring(6));
            }
            catch (Exception)
            {
                result[2] = false;
            }

            if (areaCode < 201 || areaCode > 999 || areaCode == 800 || areaCode == 877 || areaCode == 900 || areaCode == 976)
                result[0] = false;

            if (prefix < 200 || prefix > 999 || prefix == 555 || prefix == 411 || prefix == 611)
                result[1] = false;

            if (suffix > 9999)
                result[2] = false;

            return result;
        }

        /// <summary>
        /// Verifies that a Phone Extention is valid
        /// </summary>
        /// <param name="text">Extension to verify</param>
        /// <returns>True if the phone extention is valid</returns>
        public static bool ValidPhoneExt(String ext)
        {
            bool result = true;

            if (IsEmpty(ext))
                return result;

            try
            {
                int i = Int32.Parse(ext);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// checks if a year/month/date array resolves to a valid date
        /// </summary>
        /// <param name="date">String array containing year, month and date</param>
        /// <returns>An array of booleans indicating which elements passed</returns>
        public static bool[] ValidDate(String[] date)
        {
            bool[] result = { true, true, true };
            int year = 0;
            int month = 0;
            int day = 0;

            try
            {
                year = Int32.Parse(date[0]);
            }
            catch (Exception)
            {
                result[0] = false;
            }

            try
            {
                month = Int32.Parse(date[1]);
            }
            catch (Exception)
            {
                result[1] = false;
            }

            try
            {
                day = Int32.Parse(date[2]);
            }
            catch (Exception)
            {
                result[2] = false;
            }

            if (year < 1880 || year > 1999)
                result[0] = false;

            if (month < 1 || month > 12)
                result[1] = false;

            if (day < 1 || day > 31)
                result[2] = false;

            return result;
        }

        /// <summary>
        /// Checks the validity of a string based on specified criteria
        /// </summary>
        /// <param name="data">The data to verify</param>
        /// <param name="alpha">True if alpha characters are valid</param>
        /// <param name="numeric">True if numbers are valid</param>
        /// <param name="other">undelimited list of other valid characters</param>
        /// <param name="forbidden">
        /// undelimited list of other invalid characters. 
        /// If provided, alpha, numeric, and other parameters are ignored.
        /// </param>
        /// <param name="required">True if an empty or null value is invalid</param>
        /// <returns>True if all checks pass.</returns>
        public static bool CheckData(String data, bool alpha, bool numeric, String other, String forbidden, bool required)
        {
            if (required)
            {
                if (IsEmpty(data))
                    return false;
            }
            else
            {
                if (IsEmpty(data))
                    return true;
            }

            char[] c = data.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                bool match = false;
                if (alpha)
                {
                    if ((c[i] > 64 && c[i] < 91) ||
                        (c[i] > 96 && c[i] < 123) ||
                        (c[i] >= 128 && c[i] <= 165) ||
                        (c[i] >= 229 && c[i] <= 244) ||
                        c[i] == 167)
                        match = true;
                }
                if (numeric)
                {
                    if (c[i] > 47 && c[i] < 58)
                        match = true;
                }
                if (!IsEmpty(other))
                {
                    char[] cc = other.ToCharArray();
                    for (int o = 0; o < cc.Length; o++)
                    {
                        if (c[i] == cc[o])
                        {
                            match = true;
                            break;
                        }
                    }
                }

                if (!IsEmpty(forbidden))
                {
                    match = true;
                    char[] cc = forbidden.ToCharArray();
                    for (int o = 0; o < cc.Length; o++)
                    {
                        if (c[i] == cc[o])
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if (!match)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates a Canadian postal code.
        /// </summary>
        /// <param name="code">postalCode</param>
        /// <returns>True if the postal code is valid.</returns>
        public static bool ValidCanadaPostalCode(string postalCode)
        {
            return (postalCode != "" && postalCode == Regex.Match(postalCode, "[A-z]{1}[0-9]{1}[A-z]{1}[ ]{0,1}[0-9]{1}[A-z]{1}[0-9]{1}").ToString());
        }

        /// <summary>
        /// Validates a US zipcode against a state
        /// </summary>
        /// <param name="code">ZipCode</param>
        /// <param name="state">US State</param>
        /// <returns>True if the zipcode is valid within the specified state</returns>
        public static bool ValidUSPostalCode(String code, String state)
        {

            if (IsEmpty(code))
                return false;

            code = code.Replace(" ", "");

            try
            {
                code = code.Replace("-", "").Substring(0, 5);
                int test = Int32.Parse(code);
            }
            catch (Exception)
            {
                return false;
            }

            DataConnection conn = null;
            bool find = false;

            try
            {
                conn = new DataConnection();
                SqlDataReader dr;
                if (ConfigurationSettings.AppSettings.Get("commonDB") != null && ConfigurationSettings.AppSettings.Get("commonDB").Length > 0)
                    dr = conn.Execute("select zip_code from " + ConfigurationSettings.AppSettings.Get("commonDB") + ".dbo.zipcodes (nolock) where state = '" + DataConnection.Escape(state) + "' and zip_code like '" + code + "'");
                else
                    dr = conn.Execute("select zip_code from zipcodes (nolock) where state = '" + DataConnection.Escape(state) + "' and zip_code like '" + code + "'");

                if (dr.Read())
                    find = true;
            }
            catch (LoggableException le)
            {
                throw le;
            }
            catch (Exception e2)
            {
                NameValueCollection detail = new NameValueCollection(2);
                detail.Add("zip", code);
                detail.Add("state", state);
                throw new LoggableException(e2, detail);
            }
            finally
            {
                conn.CloseConnection();
            }

            return find;
        }

        /// <summary>
        /// Converts a String IP to an Integer
        /// </summary>
        /// <param name="ip">IP String</param>
        /// <returns>Integer representation of IP</returns>
        public static long ConvertIpToInt(String ip)
        {
            // validate the ip.  if invalid, return -1
            try
            {
                IPAddress.Parse(ip);
            }
            catch (Exception)
            {
                return -1;
            }

            try
            {
                long check = (long)Int32.Parse(ip);
                return check;
            }
            catch (Exception)
            {
            }

            char[] c = { '.' };
            String[] octets = ip.Split(c, 4);
            try
            {
                return ((long)((Int32.Parse(octets[0]) * 16777216L)) + (long)((Int32.Parse(octets[1]) * 65536L)) + (long)((Int32.Parse(octets[2]) * 256L)) + (long)(Int32.Parse(octets[3]))) - 2147483648L;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// Convert int ip to actual ip.
        /// Reference function [dbo].[INT_TO_IP] in common DB
        /// </summary>
        /// <param name="intAddress"></param>
        /// <returns>Actual ip</returns>
        public static string ConvertIntToIP(int intAddress)
        {
            var tmpLong = intAddress + 2147483648;
            var ip = string.Empty;
            while (tmpLong > 0)
            {
                var tmpInt = tmpLong % 256;
                if (!string.IsNullOrEmpty(ip))
                    ip = string.Concat(".", ip);
                ip = string.Concat(tmpInt.ToString(), ip);
                tmpLong = (tmpLong - @tmpInt) / 256;
            }

            return ip;
        }
        /// <summary>
        /// Checks whether the provided text is a valid domain name
        /// </summary>
        /// <param name="text">is the string to test</param>
        /// <returns>True if the string is a valid domain name; False if not</returns>
        public static bool IsValidDomain(string text)
        {
            bool isGood = false;
            Regex re = new Regex(@"(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{1,63}(?<!-)\.)+[a-zA-Z]{2,63}\.?$)");
            Regex re2 = new Regex(@"(\.ico|\.xml|\.html|\.asp|\.aspx|\.pl|\.exe|\.swf|\.jpg|\.jpeg|\.gif|\.htm|\.php|\.png|\.mp3|\.mp4|\.dll|\.pdf|\.rar|\.js|\.cgi|\.url|\.jhtml|\.config|\.torrent|\.doc|\.xls|\.csv|\.xlsx|\.iso|\.msi|\.ttf|\.config|\.blank|\.flv|\.dat|\.)$");
            Match m = re.Match(text);
            if (m.Success)
            {
                isGood = true;
                Match m2 = re2.Match(text);
                if (m2.Success)
                    isGood = false;
            }
            return isGood;
        }
    }
}
