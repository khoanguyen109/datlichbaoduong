using System;
using System.Globalization;

namespace yellowx.Framework.Extensions
{
    public static class DateTimeExtensions
    {
        private const string SECOND_AGO_FORMAT = "{0} ago";
        private const string MINUTE_AGO_FORMAT = "{0} ago";
        private const string HOUR_AGO_FORMAT = "{0} ago";
        private const string DAY_AGO_FORMAT = "{0} ago";
        private const string MONTH_AGO_FORMAT = "{0} ago";
        private const string YEAR_AGO_FORMAT = "{0} ago";

        public static string ToBirthday(this DateTime? datetime)
        {
            return datetime.ToHumanString("dd/MM/yyyy");
        }

        public static string ToHumanString(this DateTime? datetime, string languageFormat)
        {
            if (!datetime.HasValue) return "N/A";

            return datetime.Value.ToString(languageFormat);
        }
    }
}
