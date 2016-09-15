using System;
using System.IO;
using PreMailer;
using System.Web;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MaintenanceSchedule.Library.Utilities
{
    public enum MailStatus
    {
        None, Success = 1, Fail = 2, Error = 3
    }

    public class Mail
    {
        private static string imagePath = string.Empty;

        private static string GenerateBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body><img src=\"cid:Pic\"></body></html>");
            return sb.ToString();
        }

        private static string CreateEmailBody(Dictionary<string, string> contentDictionary)
        {
            string body = string.Empty;
            //using streamreader for reading my htmltemplate   
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Html/Template/thongbaodatlichbaoduong.html")))
            {
                body = reader.ReadToEnd();
            }

            foreach (var content in contentDictionary)
            {
                var name = content.Key;
                var text = content.Value;

                body = body.Replace("{" + name + "}", text);
            }
            return body;
        }

        public static MailStatus Send(string mailFrom, string mailTo, Dictionary<string, string> contentDictionary, string subject = null)
        {
            return Send("", mailFrom, mailTo, new string[] { }, contentDictionary, subject = null);
        }

        public static MailStatus Send(string mailFrom, string mailTo, string[] ccMail, Dictionary<string, string> contentDictionary, string subject = null)
        {
            return Send("", mailFrom, mailTo, ccMail, contentDictionary, subject = null);
        }

        public static MailStatus Send(string imageBase64String, string mailFrom, string mailTo, string[] ccMail, Dictionary<string, string> contentDictionary, string subject = null)
        {
            var body = CreateEmailBody(contentDictionary);
            return Send(imageBase64String, mailFrom, mailTo, ccMail, subject = null, body);
        }

        public static MailStatus Send(string mailFrom, string mailTo, string subject = null, string body = null)
        {
            return Send("", mailFrom, mailTo, new string[] { }, subject, body);
        }

        public static MailStatus Send(string mailFrom, string mailTo, string[] ccMail, string subject = null, string body = null)
        {
            return Send("", mailFrom, mailTo, ccMail, subject, body);
        }

        public static MailStatus Send(string imageBase64String, string mailFrom, string mailTo, string subject = null, string body = null)
        {
            return Send(imageBase64String, mailFrom, mailTo, new string[] { }, subject, body);
        }

        public static MailStatus Send(string imageBase64String, string mailFrom, string mailTo, string[] ccMail, string subject = null, string body = null)
        {
            var bodyContent = GenerateBody();
            try
            {
                var mailFromAddress = new MailAddress(mailFrom, mailFrom, Encoding.UTF8);
                var mailToAddress = new MailAddress(mailTo, mailTo, Encoding.UTF8);

                var mailMessage = new MailMessage(mailFromAddress, mailToAddress)
                {
                    IsBodyHtml = true,
                    Subject = subject ?? "Đặt lịch bảo dưỡng",
                    Body = body ?? string.Empty,
                    SubjectEncoding = Encoding.UTF8,
                    Priority = MailPriority.High,
                };

                foreach (var cc in ccMail)
                    mailMessage.CC.Add(cc);
                
                //var contentId = "Pic";
                //var imageData = Convert.FromBase64String(imageBase64String);
                //var linkedResource = new LinkedResource(new MemoryStream(imageData), "image/jpeg")
                //{
                //    ContentId = contentId,
                //    TransferEncoding = TransferEncoding.Base64
                //};

                ////attach error image
                //AlternateView view = AlternateView.CreateAlternateViewFromString(bodyContent, null, MediaTypeNames.Text.Html);
                //view.LinkedResources.Add(linkedResource);
                //mailMessage.AlternateViews.Add(view);

                using (var clientMail = new SmtpClient { EnableSsl = true, Timeout = 120000 })
                {
                    try
                    {
                        clientMail.Send(mailMessage);
                        return MailStatus.Success;
                    }
                    catch (Exception ex)
                    {
                        //Somehow, send mail time out because many files attached.
                        //So send mail again without image attached.
                        //var detail = new NameValueCollection();
                        //detail.Add("attachedFiles", testResultFile);
                        //new LoggableException(ex, detail);

                        using (var errMailMessage = new MailMessage(mailFrom, mailTo)
                        {
                            Subject = "Send fail.",
                            IsBodyHtml = true,
                            Priority = MailPriority.High,
                            Body = body
                        })
                        {
                            clientMail.Send(errMailMessage);
                        }
                        return MailStatus.Error;
                    }
                }
            }
            catch (System.Net.Mail.SmtpException exSmtp)
            {
                //nameValueCollection = new NameValueCollection(3);
                //nameValueCollection.Add("SMTP Exception Error", exSmtp.Message);
                //nameValueCollection.Add("Send Email To", mailTo);
                //nameValueCollection.Add("Email Message", bodyContent);
                //new LoggableException(exSmtp, nameValueCollection);

                //statusSend = string.Concat("Exception occurred:", exSmtp.Message);
                return MailStatus.Error;
            }
            catch (System.Exception exGen)
            {
                //nameValueCollection = new NameValueCollection(3);
                //nameValueCollection.Add("General Exception Error", exGen.Message);
                //nameValueCollection.Add("Send Email To", mailTo);
                //nameValueCollection.Add("Email Message", bodyContent);
                //new LoggableException(exGen, nameValueCollection);

                //statusSend = string.Concat("Exception occurred:", exGen.Message);
                return MailStatus.Error;
            }
        }
    }
}
