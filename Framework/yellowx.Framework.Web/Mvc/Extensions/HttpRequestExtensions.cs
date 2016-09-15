using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace yellowx.Framework.Web.Mvc.Extensions
{
    public static class HttpRequestExtensions
    {
        public static int GetInteger(this HttpRequestBase request, string fieldName)
        {
            return request[fieldName] != null ? Convert.ToInt32(request[fieldName]) : 0;
        }

        public static string GetString(this HttpRequestBase request, string fieldName)
        {
            return request[fieldName];
        }

    }
}
