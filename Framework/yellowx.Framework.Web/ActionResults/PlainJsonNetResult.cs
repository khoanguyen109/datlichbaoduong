using System.Web.Mvc;

namespace yellowx.Framework.Web.ActionResults
{
    public class PlainJsonNetResult : JsonNetResult
    {
        public PlainJsonNetResult(object data, bool allowGet = false)
        {
            Data = data;
            if (allowGet)
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
    }
}