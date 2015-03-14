using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SNMS_DataService.Database
{
    class DatabaseGateway
    {
        private static DatabaseGateway instance;

        string m_sMySqlServerHost;
        string m_sUserName;
        string m_sPassword;
        string m_sDbName;
        MySqlConnection m_mySqlConnection;
        Mutex m_mutex;

        public class DatabaseParameters
        {
            public string m_sHost;
            public string m_sUser;
            public string m_sPass;
            public string m_sDbName;

            public DatabaseParameters(string sHost, string sUser, string sPass, string sDbName)
            {
                m_sHost = sHost;
                m_sUser = sUser;
                m_sPass = sPass;
                m_sDbName = sDbName;
            }
        }

        private DatabaseGateway(DatabaseParameters parameters)
        {
            m_sMySqlServerHost = parameters.m_sHost;
            m_sUserName = parameters.m_sUser;
            m_sPassword = parameters.m_sPass;
            m_sDbName = parameters.m_sDbName;
            m_mySqlConnection = null;
            m_mutex = new Mutex();
        }

        public static DatabaseGateway Instance(DatabaseParameters parameters)
        {
            if (instance == null)
            {
                if (parameters == null)
                {
                    return null;
                }
                instance = new DatabaseGateway(parameters);
            }
            return instance;
        }

        void Connect()
        {
            string sConnectionString =  "server="       +   m_sMySqlServerHost  + 
                                        ";uid="         +   m_sUserName         +
                                        ";pwd="         +   m_sPassword         +
                                        ";database="    +   m_sDbName           +
                                        ";";

            if (m_mySqlConnection != null)
            {
                try
                {
                    m_mySqlConnection.Close();
                }
                catch (Exception e)
                {
                }
            }

            m_mySqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
            m_mySqlConnection.ConnectionString = sConnectionString;
            m_mySqlConnection.Open();
        }

        void PrepareForQuery()
        {
            if (m_mySqlConnection == null || m_mySqlConnection.Ping() == false)
            {
                Connect();
            }
        }

        public MySqlDataReader ReadQuery(string sQuery)
        {
            m_mutex.WaitOne();
            try
            {
                PrepareForQuery();

                MySqlDataReader reader;
                MySqlCommand cmd = m_mySqlConnection.CreateCommand();
                cmd.CommandText = sQuery;
                reader = cmd.ExecuteReader();

                return reader;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                m_mutex.ReleaseMutex();
            }
        }

        public int WriteQuery(string sQuery)
        {
            m_mutex.WaitOne();
            try
            {
                PrepareForQuery();

                MySqlCommand cmd = m_mySqlConnection.CreateCommand();
                cmd.CommandText = sQuery;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return 0;
            }
            finally
            {
                m_mutex.ReleaseMutex();
            }
        }

        void BuildDb()
        {

        }
    }
}
