using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yellowx.Framework.Web.Cookies
{
    public interface ICookieIdentity
    {
        string Id { get; set; }
        string ClientId { get; set; }
    }

    public class CookieIdentity : ICookieIdentity
    {
        public string Id
        {
            get; set;
        }

        public string ClientId
        {
            get; set;
        }
    }
}
