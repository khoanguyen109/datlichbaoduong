using Owin;
using System;
using Microsoft.Owin;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;

[assembly: OwinStartupAttribute(typeof(MaintenanceSchedule.Startup))]
namespace MaintenanceSchedule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Common/Account/SignIn"),
                CookieSecure = CookieSecureOption.SameAsRequest,
                ExpireTimeSpan = TimeSpan.FromMinutes(30)
            });
        }
    }
}
