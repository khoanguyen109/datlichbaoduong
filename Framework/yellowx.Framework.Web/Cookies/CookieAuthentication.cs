using yellowx.Framework.Identity;

namespace yellowx.Framework.Web.Cookies
{
    public class CookieAuthentication : Authentication<CookieIdentity>
    {
        public override CookieIdentity SignIn(string email)
        {
            return base.SignIn(email);
        }

        public override CookieIdentity GetCookieIdentity()
        {
            return base.GetCookieIdentity();
        }

    }
}
