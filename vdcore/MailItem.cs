using System;
using System.Xml;
using System.Collections.Specialized;
using Vendare.Error;

namespace Vendare.Mail
{
	/// <summary>
	/// Encapsulates a single email message template.
	/// These are stored in XML format in a dile specified if the mailFile
	/// App Setting. MailItems are created and maintained by the MailCache.
	/// </summary>
	public class MailItem
	{
		private String fromName;
		private String fromEmail;
		private String subject;
		private String body = "";
		private String htmlBody = "";
		private String id;

		public MailItem()
		{
		}

		public MailItem(XmlNode node) 
		{
			try 
			{
				id =  node.Attributes["id"].Value;
				fromName = node.SelectSingleNode("fromName").InnerText;
				fromEmail = node.SelectSingleNode("fromEmail").InnerText;
				subject = node.SelectSingleNode("subject").InnerText;
				if(node.SelectSingleNode("body") != null)
					body = node.SelectSingleNode("body").InnerText;
				if(node.SelectSingleNode("htmlBody") != null)
					htmlBody = node.SelectSingleNode("htmlBody").InnerXml;
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(1);
				if(node != null)
					detail.Add("nodeText", node.InnerText);
				throw new LoggableException(e, detail);
			}
		}

		public String FromName 
		{
			get { return fromName; }
		}

		public String FromEmail
		{
			get { return fromEmail; }
		}
		public String Subject 
		{
			get { return subject; }
		}
		public String Body 
		{
			get { return body; }
		}
		public String HtmlBody 
		{
			get { return htmlBody; }
		}
		public String ID 
		{
			get { return id; }
		}
	}
}
