using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using Vendare.Error;

namespace Vendare.Mail
{
	/// <summary>
	/// Maintains a cache of email items stored in an xml file.
	/// If this file is updated, the cache will refresh itself.
	/// </summary>
	public class MailCache
	{
		// Single static instance obtained through getInstance()
		private static MailCache cache = null;
    
		// Reference to mail xml file containing mail content
		private String mailFile = ConfigurationSettings.AppSettings.Get("mailFile");
    
		//Time that class was last refreshed with info in mail file
		private DateTime lastRefresh;

		//hashtable for individual mail items
		private Hashtable items = null;

		private FileSystemWatcher watcher;

		/// <summary>
		/// For obtaining the Mail Cache
		/// </summary>
		/// <returns>Returns the single static cache instance.</returns>
		public static MailCache GetInstance() 
		{
			lock(typeof(MailCache)) 
			{
				if(cache==null) 
					cache = new MailCache();
			}

			return cache;
		}
    
		private void Close() 
		{
			if(watcher != null)
				watcher.Dispose();
			watcher = null;

			GC.SuppressFinalize(this);
		}

		~ MailCache() 
		{
			Close();
		}

		public static void ShutDown(Object sender, EventArgs args)
		{
			if(cache != null)
				cache.Close();
		}

		/// <summary>
		/// Creates an MailCache instance. This can only be called through 
		/// MailCache.getInstance(). Upon instantiation, refresh() is called to
		/// update internal state with the mailFile.
		/// </summary>
		private MailCache() 
		{
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(MailCache.ShutDown);
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(MailCache.ShutDown);

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\mail.xml"))
            {
                watcher = new FileSystemWatcher();
                if (mailFile == null || mailFile.Length == 0)
                    mailFile = AppDomain.CurrentDomain.BaseDirectory + "\\mail.xml";
                int idx = mailFile.LastIndexOf("\\");
                watcher.Path = mailFile.Substring(0, idx);
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = mailFile.Substring(idx + 1);

                // Add event handlers.
                watcher.Changed += new FileSystemEventHandler(OnMailFileChanged);

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                refresh();
            }

		}

		private void OnMailFileChanged(object source, FileSystemEventArgs e)
		{
			refresh();
		}

		/// <summary>
		/// Synchronizes internal state with information in mailFile
		/// </summary>
		public void refresh() 
		{
			lock(this) 
			{
				try 
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(mailFile);
					XmlNodeList list = doc.GetElementsByTagName("mailItem");
					items = new Hashtable();

					foreach(XmlNode node in list) 
						items.Add(node.Attributes["id"].Value, new MailItem(node));

					lastRefresh = DateTime.Now;
				}
				catch(LoggableException le) 
				{
					throw le;
				}
				catch(Exception e) 
				{
					throw new LoggableException(e, null);
				}
			}
		}
    
		public MailItem GetMailItem(String id) 
		{
			try 
			{
				return (MailItem)items[id];
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("id", id);
				throw new LoggableException(e, detail);
			}
		}
	}
}
