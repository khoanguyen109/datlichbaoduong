using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaintenanceSchedule.Library.Utilities
{
    public class Constant
    {
        // format
        public const string FORMAT_DATETIME = "dd/MM/yyyy";

        // symbols
        public const string VND = "VNĐ";
        public const string PERCENTAGE_SYMBOL = "%";
        public const char COLON_SYMBOL_CHAR = ':';
        public const char COLON_SYMBOL_COMMA = ',';
        public const char COLON_SYMBOL_DASH = '-';
        public const char COLON_SYMBOL_SLASH = '/';
        public const char COLON_SYMBOL_SPACE = ' ';
    }

    public static class Characters
    {
        public static string Chop(string s, int length)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException(s);
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words[0].Length > length)
                return words[0];
            var sb = new StringBuilder();

            foreach (var word in words)
            {
                if ((sb + word).Length > length)
                    return string.Format("{0}...", sb.ToString().TrimEnd(' '));
                sb.Append(word + " ");
            }
            return string.Format("{0}...", sb.ToString().TrimEnd(' '));
        }

        public static string ConvertToUnSign3(string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var temp = str.ToLower().Normalize(NormalizationForm.FormD);
            var replacedUnSign = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            var returnStr = string.Join(Constant.COLON_SYMBOL_DASH.ToString(), replacedUnSign.Split(Constant.COLON_SYMBOL_SPACE));
            return returnStr;
        }

        public static Dictionary<char, int> CharacterCount(this string text)
        {
            return text.GroupBy(c => c)
                       .OrderBy(c => c.Key)
                       .ToDictionary(grp => grp.Key, grp => grp.Count());
        }

        public static string DecodeFromUtf8(string utf8String)
        {
            byte[] bytes = Encoding.Default.GetBytes(utf8String);
            utf8String = Encoding.UTF8.GetString(bytes);
            return utf8String;
        }

        public static string GetStringWithoutHttp(string url)
        {
            System.Uri uri = new Uri(url);
            string uriWithoutScheme = uri.Host + uri.PathAndQuery;
            return uriWithoutScheme.Replace("www.", "").TrimEnd('/');
        }
    }
}
