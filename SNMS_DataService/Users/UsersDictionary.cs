using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


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
    }
}
