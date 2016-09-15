using System.Collections.Generic;
using System.Web.Optimization;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Web.Mvc.Bundles
{
    public class BundleFactory
    {
        private const string BundleVirtualPath = "~/bundles/{0}/{1}";

        public static Bundle GetBundle(string name, BundleType type)
        {
            return type == BundleType.Script ? scriptBundles[name] : styleBundles[name];
        }

        private static readonly Dictionary<string, Bundle> scriptBundles;
        private static readonly Dictionary<string, Bundle> styleBundles;

        static BundleFactory()
        {
            scriptBundles = new Dictionary<string, Bundle>();
            styleBundles = new Dictionary<string, Bundle>();
        }

        public static void CreateBundle(string name, BundleType type, params string[] virtualPaths)
        {
            var virtualPath = BundleVirtualPath.FormatWith(name, type.ToString()).ToLower();
            switch (type)
            {
                case BundleType.Script:
                    if (!virtualPaths.IsNullOrEmpty())
                    {
                        var scriptBundle = new ScriptBundle(virtualPath).Include(virtualPaths);
                        scriptBundles.Add(name, scriptBundle);
                        BundleTable.Bundles.Add(scriptBundle);
                    }
                    break;
                default:
                    if (!virtualPaths.IsNullOrEmpty())
                    {
                        var styleBundle = new StyleBundle(virtualPath).Include(virtualPaths);
                        styleBundles.Add(name, styleBundle);
                        BundleTable.Bundles.Add(styleBundle);
                    }
                    break;
            }
        }
    }
    public enum BundleType
    {
        Script,
        Style
    }
}
