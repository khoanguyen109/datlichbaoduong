using System.Web.Mvc;

namespace yellowx.Framework.Web.Mvc
{
    public class PermissionAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly string name;
        private readonly string description;

        public PermissionAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {

        }
    }
}
