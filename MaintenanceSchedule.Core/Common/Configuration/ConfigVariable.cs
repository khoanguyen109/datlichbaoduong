using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MaintenanceSchedule.Core.Common.Configuration
{
    public class ConfigVariable
    {
        public static string MailFrom
        {
            get {
                var mailFrom = ConfigurationManager.AppSettings["mailFrom"];
                return !string.IsNullOrEmpty(mailFrom) ? mailFrom.ToString() : string.Empty;
            }
        }

        public static string MailTo
        {
            get
            {
                var mailTo = ConfigurationManager.AppSettings["mailTo"];
                return !string.IsNullOrEmpty(mailTo) ? mailTo.ToString() : string.Empty;
            }
        }

        public static string Cc
        {
            get
            {
                var cc = ConfigurationManager.AppSettings["cc"];
                return !string.IsNullOrEmpty(cc) ? cc.ToString() : string.Empty;
            }
        }

        public static string PrimaryDomainUrl
        {
            get
            {
                var primaryDomainUrl = ConfigurationManager.AppSettings["primaryDomainUrl"];
                return !string.IsNullOrEmpty(primaryDomainUrl) ? primaryDomainUrl.ToString() : string.Empty;
            }
        }

        public static string MainDomainUrl
        {
            get
            {
                var mainDomainUrl = ConfigurationManager.AppSettings["mainDomainUrl"];
                return !string.IsNullOrEmpty(mainDomainUrl) ? mainDomainUrl.ToString() : string.Empty;
            }
        }

        public static string ServiceDirectoryUrl
        {
            get
            {
                var serviceDirectoryUrl = ConfigurationManager.AppSettings["serviceDirectoryUrl"];
                return !string.IsNullOrEmpty(serviceDirectoryUrl) ? serviceDirectoryUrl.ToString() : string.Empty;
            }
        }

        public static string RelativeServiceUrl
        {
            get
            {
                var relativeServiceUrl = ConfigurationManager.AppSettings["relativeServiceUrl"];
                return !string.IsNullOrEmpty(relativeServiceUrl) ? relativeServiceUrl.ToString() : string.Empty;
            }
        }

        public static string RelativeServiceMediumUrl
        {
            get
            {
                var relativeServiceMediumUrl = ConfigurationManager.AppSettings["relativeServiceMediumUrl"];
                return !string.IsNullOrEmpty(relativeServiceMediumUrl) ? relativeServiceMediumUrl.ToString() : string.Empty;
            }
        }

        public static string RelativeServiceThumbUrl
        {
            get
            {
                var relativeServiceThumbUrl = ConfigurationManager.AppSettings["relativeServiceThumbUrl"];
                return !string.IsNullOrEmpty(relativeServiceThumbUrl) ? relativeServiceThumbUrl.ToString() : string.Empty;
            }
        }

        public static string Name
        {
            get
            {
                var name = ConfigurationManager.AppSettings["name"];
                return !string.IsNullOrEmpty(name) ? name.ToString() : string.Empty;
            }
        }

        public static string Email
        {
            get
            {
                var email = ConfigurationManager.AppSettings["email"];
                return !string.IsNullOrEmpty(email) ? email.ToString() : string.Empty;
            }
        }

        public static string Password
        {
            get
            {
                var password = ConfigurationManager.AppSettings["password"];
                return !string.IsNullOrEmpty(password) ? password.ToString() : string.Empty;
            }
        }
    }
}
