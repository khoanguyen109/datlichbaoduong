using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Vendare.Error;

namespace Vendare.DBAccess
{
    public class LuTable : Hashtable
    {
        private string tablename;
        private string keycolumn;
        private string valuecolumn;

        public LuTable(string tablename, string keycolumn, string valuecolumn)
        {
            this.tablename = tablename;
            this.keycolumn = keycolumn;
            this.valuecolumn = valuecolumn;

            Refresh();
        }

        public bool Refresh()
        {
            bool result = false;
            SqlConnection conn = null ;
            SqlCommand cmd = null;

            try
            {
                using (conn = new SqlConnection(ConfigurationManager.AppSettings.Get("dsn")))
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT " + keycolumn + ", " + valuecolumn + " FROM " + tablename, conn);
                    cmd.CommandType = CommandType.Text;

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        this.Add(reader[keycolumn], reader[valuecolumn]);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                NameValueCollection detail = new NameValueCollection();
                detail.Add("keycolumn", keycolumn);
                detail.Add("valuecolumn", valuecolumn);
                detail.Add("tablename", tablename);
                detail.Add("connection string", conn.ConnectionString);
                detail.Add("sql command", cmd.CommandText);
                new LoggableException(ex, detail);
            }
            return result;
        }
    }
}
