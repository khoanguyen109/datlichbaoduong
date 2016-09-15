using System;
using System.Web.Mail;
using Vendare.Error;
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;
using Vendare.DBAccess;
using System.Data;
using System.Data.SqlClient;

namespace Vendare.Mail
{

	/// <summary>
	/// Provides a static method for sending mail.
	/// </summary>
	public class Mailer
	{
		private static MailCache cache = MailCache.GetInstance();

		public static void Send(String mailId, String toName, ArrayList toEmails, NameValueCollection substitutions) 
		{
			foreach(String email in toEmails)
			{
				Send(mailId, toName, email, substitutions);
			}	
		}

		public static void Send(String mailId, String toName, ArrayList toEmails, NameValueCollection substitutions, bool isSubscription)
		{
			foreach(String email in toEmails)
			{
				Send(mailId, toName, email, substitutions, isSubscription);
			}
		}

		public static void Send(String mailId, String toName, String toEmail, NameValueCollection substitutions) 
		{
			Send(mailId, toName, toEmail, substitutions, false);
		}

		public static void SendDirect(String mailId, String toName, String toEmail, NameValueCollection substitutions) 
		{
			MailMessage email = new MailMessage();
			MailItem item = cache.GetMailItem(mailId);
			email.To = "\"" + toName + "\" <" + toEmail + ">";
			email.From = "\"" + item.FromName + "\" <" + item.FromEmail + ">";
			email.Subject = Substitute(item.Subject, substitutions, false);
			if(item.HtmlBody.Length == 0)
				email.Body = Substitute(item.Body, substitutions, false);
			else 
			{
				email.BodyFormat = MailFormat.Html;
				email.Body = Substitute(item.HtmlBody, substitutions, false);
			}
			if(ConfigurationSettings.AppSettings["smtpServer"] != null)
				SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["smtpServer"];
			else
				SmtpMail.SmtpServer = "localhost";
			try 
			{
				SmtpMail.Send(email);
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(2);
				detail.Add("to",email.To);
				detail.Add("from", email.From);
				while(e.InnerException != null)
					e = e.InnerException;
				new LoggableException(e, detail);
			}
		}

		public static void SendAlerts(String mailId, String toName, String toEmail, String fromName, String fromEmail, String eSubject, String eText, String hText) 
		{
			MailMessage email = new MailMessage();
			email.To = "\"" + toName + "\" <" + toEmail + ">";
			email.From = "\"" + fromName + "\" <" + fromEmail + ">";
			email.Subject = eSubject;
			if (hText == "" || hText == null)
				email.Body = eText;
			else
			{
				email.BodyFormat = MailFormat.Html;
				email.Body = hText;
			}

			if(ConfigurationSettings.AppSettings["smtpServer"] != null)
				SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["smtpServer"];
			else
				SmtpMail.SmtpServer = "localhost";
			try 
			{
				SmtpMail.Send(email);
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(2);
				detail.Add("to",email.To);
				detail.Add("from", email.From);
				while(e.InnerException != null)
					e = e.InnerException;
				new LoggableException(e, detail);
			}
		}

		public static void Send(String mailId, String toName, String toEmail, NameValueCollection substitutions, bool subscription) 
		{
			MailItem item = cache.GetMailItem(mailId);
			Send(mailId, toName, toEmail, item.FromName, item.FromEmail, substitutions, subscription);
		}

		public static void Send(String mailId, String toName, String toEmail, String fromName, String fromEmail, NameValueCollection substitutions, bool subscription) 
		{
			DataConnection conn = null;
			try 
			{
				MailItem item = cache.GetMailItem(mailId);

				conn = new DataConnection();

				String proc;
				if(subscription)
					proc="sp_insert_outgoing_email_subscription";
				else
					proc="sp_insert_outgoing_email";
				
				SqlCommand cmd = conn.GetProcCommand(proc);
				cmd.Parameters.Add("@recip_name",SqlDbType.VarChar,40).Value = toName;
				cmd.Parameters.Add("@recip_email",SqlDbType.VarChar,40).Value = toEmail;
				cmd.Parameters.Add("@sender_name",SqlDbType.VarChar,40).Value = fromName;
				cmd.Parameters.Add("@sender_email",SqlDbType.VarChar,40).Value = fromEmail;
				cmd.Parameters.Add("@email_subject",SqlDbType.VarChar,300).Value = Substitute(item.Subject, substitutions, false);
				cmd.Parameters.Add("@email_text",SqlDbType.Text).Value = Substitute(item.Body, substitutions, false);
				cmd.Parameters.Add("@email_html",SqlDbType.Text).Value = Substitute(item.HtmlBody, substitutions, true);
				if(subscription)
					cmd.Parameters.Add("@target_server",SqlDbType.Int).Value = 51;
				cmd.ExecuteNonQuery();				
			}
			catch(LoggableException le) 
			{
				throw le;
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(3);
				detail.Add("mailId", mailId);
				detail.Add("toName", toName);
				detail.Add("toEmail", toEmail);
				throw new LoggableException(e, detail);
			}
			finally 
			{
				if(conn != null)
					conn.CloseConnection();
			}
		}

		private static String Substitute(String body, NameValueCollection subs, bool forHtml) 
		{
			String[] keys = subs.AllKeys;
			for(int i=0; i < keys.Length; i++) 
			{
				body = body.Replace("%" + keys[i] + "%",subs[keys[i]]);
				if(forHtml)
					body = body.Replace("\n","<br>");
			}

			return body;
		}

	}

}
