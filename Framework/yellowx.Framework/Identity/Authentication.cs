using System;

namespace yellowx.Framework.Identity
{
    public interface IAuthentication<TIdentity>
    {
        TIdentity SignIn(string email);
        TIdentity SignOut(string sessionId);
        TIdentity SignInGuest(string sessionId);
        TIdentity GetCookieIdentity();
    }

    public class Authentication<TIdentity> : IAuthentication<TIdentity>
    {
        public virtual TIdentity GetCookieIdentity()
        {
            throw new NotImplementedException();
        }

        public virtual TIdentity SignIn(string email)
        {
            throw new NotImplementedException();
        }

        public virtual TIdentity SignInGuest(string sessionId)
        {
            throw new NotImplementedException();
        }

        public virtual TIdentity SignOut(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
