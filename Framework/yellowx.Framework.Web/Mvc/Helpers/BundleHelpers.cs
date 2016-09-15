using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using yellowx.Framework.Web.Mvc.Bundles;

namespace yellowx.Framework.Web.Mvc.Helpers
{
    public static class BundleExtensions
    {
        public static IHtmlString Bundle(this HtmlHelper helper, string name)
        {
            var scriptBundle = BundleFactory.GetBundle(name, BundleType.Script);
            var styleBundle = BundleFactory.GetBundle(name, BundleType.Style);
            var scriptHtml = scriptBundle != null ? Scripts.Render(scriptBundle.Path).ToString() : "";
            var styleHtml = styleBundle != null ? Styles.Render(styleBundle.Path).ToString() : "";

            //make sure style always render before script.
            var html = new HtmlString(styleHtml + scriptHtml);
            return html;
        }
    }
}
