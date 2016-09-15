using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace Vendare.Utils
{
    public class CustomTracer
    {
        static public void Write(string msg)
        {
            Write(msg, null);
        }
        static public void Write(string msg,System.Web.HttpContext ctx)
        {
            HttpContext context = HttpContext.Current;
            if(ctx != null)
                context = ctx;
            if (context != null)
                HttpContext.Current.Trace.Write(msg);
            //else   alternative logging TBD

        }
    }
}
