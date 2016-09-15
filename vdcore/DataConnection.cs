using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using Vendare.Error;
using Vendare.Utils;

namespace Vendare.DBAccess
{
	/// <summary>
	/// Basic wrapper for database calls
	/// </summary>
	public class DataConnection
	{
		/// <summary>Database Connection that this class is wrapping</summary>
		protected SqlConnection conn=null;
		protected static String dsn = ConfigurationSettings.AppSettings.Get("dsn");

		/// <summary>
		/// Creates class and sets up a new connection to the config specified dsn
		/// </summary>
		public DataConnection()
		{
			try 
			{
				conn = GetConnection();
				conn.Open();
			}
			catch(LoggableException le) 
			{
				throw le;
			}
			catch(Exception e) {
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("dsn",conn.ConnectionString);
				throw new LoggableException(e, detail);
			}
		}

		/// <summary>
		/// Creates class and sets up a new connection from the specified dsn
		/// </summary>
		public DataConnection(String dsn)
		{
			try 
			{
				conn = GetConnection(dsn);
				conn.Open();
			}
			catch(LoggableException le) 
			{
				throw le;
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("dsn",dsn);
				throw new LoggableException(e, detail);
			}
		}

		/// <summary>
		/// Returns the wrapped connection
		/// </summary>
		/// <returns>Returns the wrapped connection</returns>
		public SqlConnection GetInnerConnection() 
		{
			return conn;
		}

		/// <summary>
		/// Obtain a SqlCommand for executing a stored procedure
		/// </summary>
		/// <param name="procName">Name of stored procedure</param>
		/// <returns>A valid SqlCommand</returns>
		public SqlCommand GetProcCommand(String procName) 
		{
			try 
			{
				SqlCommand command = new SqlCommand(procName, conn);
				command.CommandType = CommandType.StoredProcedure;
				return command;
			}
			catch(Exception e)
			{
				throw new LoggableException(e, null);
			}
		}

		/// <summary>
		/// Execute a sql statement that does not return results
		/// </summary>
		/// <param name="sql">SQL to execute</param>
		/// <returns>Number of rows affected</returns>
		public int ExecuteNonQuery(String sql) 
		{
			try 
			{
				SqlCommand command = new SqlCommand(sql, conn);
				return command.ExecuteNonQuery();
			}
			catch(Exception e)
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("sql",sql);
				throw new LoggableException(e, detail);
			}
		}

        /// <summary>
        /// Execute a sql statement that returns single value
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Object Value</returns>
        public object ExecuteScalar(String sql)
        {
            try
            {
                SqlCommand command = new SqlCommand(sql, conn);
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                NameValueCollection detail = new NameValueCollection(1);
                detail.Add("sql", sql);
                throw new LoggableException(e, detail);
            }
        }

		/// <summary>
		/// Execute a sql statement that that returns results
		/// </summary>
		/// <param name="sql">SQL to execute</param>
		/// <returns>DataReader of results</returns>
		public SqlDataReader Execute(String sql) 
		{
			try 
			{
				SqlCommand command = new SqlCommand(sql, conn);
				return command.ExecuteReader();
			}
			catch(Exception e)
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("sql",sql);
				throw new LoggableException(e, detail);
			}
		}

		public DataSet ExecuteSet(String sql) 
		{
			try 
			{
				SqlDataAdapter da = new SqlDataAdapter(sql,conn);
				DataSet ds = new DataSet();
				da.Fill(ds);
				return ds;
			}
			catch(Exception e)
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("sql",sql);
				throw new LoggableException(e, detail);
			}
		}
        public DataSet ExecuteSet(SqlCommand command)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception e)
            {
                NameValueCollection detail = new NameValueCollection(1);
                detail.Add("command's sql", command.CommandText);
                throw new LoggableException(e, detail);
            }
        }

		/// <summary>
		/// Closes the connection held by this DataConnection
		/// </summary>
		public void CloseConnection() 
		{
			try 
			{
				conn.Close();
			}
			catch(Exception ) {}
		}

		~DataConnection() 
		{
			CloseConnection();
		}

		/// <summary>
		/// returns a connection from the dsn specified in .config
		/// </summary>
		public static SqlConnection GetConnection()
		{
			return GetConnection(dsn);
		}

		/// <summary>
		/// returns a connection from the dsn provided
		/// </summary>
		public static SqlConnection GetConnection(String dsn)
		{
			try
			{
				return new SqlConnection(dsn);
			}
			catch(Exception e) 
			{
				NameValueCollection detail = new NameValueCollection(1);
				detail.Add("dsn",dsn);
				throw new LoggableException(e, detail);
			}
		}

		/// <summary>
		/// This prepares strings to be included into sql statements
		/// by escaping potentially problematic characters
		/// </summary>
		/// <param name="text">text to escape</param>
		/// <returns>escaped text</returns>
		public static String Escape(String text) 
		{

			if(Helper.IsEmpty(text))
				return text;

			return text.Replace("'","''");
		}

	}
}
