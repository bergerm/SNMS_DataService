using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MySql.Data.MySqlClient;

using SNMS_DataService.Database;
using SNMS_DataService.Queries;

namespace SNMS_DataService.Users
{
    class UsersDictionary
    {
        private static UsersDictionary instance;
        Mutex m_mutex;

        Dictionary<string, User> usersDictionary;

        private UsersDictionary()
        {
            usersDictionary = new Dictionary<string, User>();
            m_mutex = new Mutex();
        }

        public static UsersDictionary Instance()
        {
            if (instance == null)
            {
                instance = new UsersDictionary();
            }
            return instance;
        }

        public void AddUser(User user)
        {
            m_mutex.WaitOne();
            usersDictionary.Add(user.GetName(), user);
            m_mutex.ReleaseMutex();
        }

        public User GetUser(string sUserName)
        {
            m_mutex.WaitOne();
            if (usersDictionary.Keys.Contains(sUserName))
            {
                User user =  usersDictionary[sUserName].Clone();
                m_mutex.ReleaseMutex();
                return user;
            }
            m_mutex.ReleaseMutex();
            return null;
        }

        public void LoadUsers()
        {
            m_mutex.WaitOne();

            usersDictionary.Clear();

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetUsersQuery());

            while (reader.Read())
            {
                int dwUserId = Int32.Parse(reader["UserID"].ToString());
                string sUserName = reader["UserName"].ToString();
                string sHashedPassword = reader["UserHashedPassword"].ToString();
                int dwUserTypeId = Int32.Parse(reader["UserTypeID"].ToString());

                byte dwUserEnableRead = byte.Parse(reader["UserEnableRead"].ToString());
                bool userEnableRead = (dwUserEnableRead == 1) ? true : false;
                int dwUserEnableWrite = byte.Parse(reader["UserEnableWrite"].ToString());
                bool userEnableWrite = (dwUserEnableWrite == 1) ? true : false;

                User user = new User(sUserName, sHashedPassword, (UserTypes)dwUserTypeId, userEnableRead, userEnableWrite);
                usersDictionary.Add(sUserName, user);
            }

            reader.Close();

            m_mutex.ReleaseMutex();
        }
    }
}
