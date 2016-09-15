using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vendare.DBAccess;
using Vendare.Error;
using Vendare.Utils;
using System.Threading;

/// <summary>
/// wrapper for getting a db connection.  if getting a db connection fails, then sleep x millisecs and
/// try again.  if greater than x attempts, then throws exception
/// best for batch/automatetd processes
/// </summary>

namespace Vendare.DBAccess
{
	public class RecursiveConnection
	{
		private String dsn = null;

		private int errorCount = 0;
		private int attempts = 10;
		private int msSleep = 30000;

		public RecursiveConnection(String dsn, int attempts, int msSleep)
		{
			this.dsn = dsn;
			this.attempts = attempts;
			this.msSleep = msSleep;
		}

		//gets a db connection.  if getting a db connection fails, then sleep 5 minutes and
		//try again.  every 1 out of 10 errors send an alert
		public DataConnection GetDataConnection()
		{
			DataConnection conn = null;
			while(conn == null)
			{
				try
				{
					conn = new DataConnection(dsn);
				}
				catch(Exception e) 
				{						
					errorCount++;
					if(errorCount > attempts)
					{
						throw e;
					}
					Thread.Sleep(msSleep);
				}
			}
			return conn;
		}

	}
}

