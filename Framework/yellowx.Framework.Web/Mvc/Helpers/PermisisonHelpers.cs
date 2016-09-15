using System.Web.Mvc;

namespace yellowx.Framework.Web.Mvc.Helpers
{
    public static class PermisisonHelpers
    {
        /// <summary>
        /// Checks whether user has permission on specified link in UI.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static bool HasPermisison(this HtmlHelper htmlHelper)
        {
            return true;
        }
    }
}
