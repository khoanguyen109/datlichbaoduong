using System;

namespace yellowx.Framework.Web.Navigation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BreadcrumbAttribute : Attribute
    {
        private readonly string key;
        private readonly string @default;

        public BreadcrumbAttribute(string key, string @default)
        {
            this.key = key;
            this.@default = @default;
        }
    }
}
