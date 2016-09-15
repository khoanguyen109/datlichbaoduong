using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using MaintenanceSchedule.Web.Controllers;
using MaintenanceSchedule.Web.Areas.Common.Models;
using MaintenanceSchedule.Core.Common.Configuration;

namespace MaintenanceSchedule.Web.Areas.Common.Controllers
{
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        public ActionResult SignIn(string email)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("SignIn", new { area = "Common" });
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(AccountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var validated = Validate(viewModel.Email, viewModel.Password);
                if (!validated)
                {
                    ModelState.AddModelError("Login Error", "Đăng nhập không thành công. Sai tên đăng nhập hay mật khẩu.");
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            HttpContext.Session.Clear();
            AuthenticationManager.SignOut();
            return RedirectToAction("SignIn", new { area = "Common" });
        }

        private bool Validate(string email, string password)
        {
            if (email == ConfigVariable.Email && password == ConfigVariable.Password)
            {
                var claims = GetClaims(ConfigVariable.Name, ConfigVariable.Email);
                if (claims != null)
                {
                    Authenticate(claims);
                    return true;
                }
            }
            return false;
        }

        private List<Claim> GetClaims(string name, string email)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void Authenticate(List<Claim> claims)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimsIdentity);
            HttpContext.User = AuthenticationManager.AuthenticationResponseGrant.Principal;
        }
    }
}
