using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMS_DataService.Users
{
    public enum UserTypes
    {
        userOperator = 1,
        userAdministrator
    }

    class User
    {
        string m_sName;
        string m_sHashedPassword;
        UserTypes m_eUserType;
        bool m_bEnableRead;
        bool m_bEnableWrite;

        public User(string sName, string sPass, UserTypes eType, bool bRead, bool bWrite)
        {
            m_sName = sName;
            m_sHashedPassword = sPass;
            m_eUserType = eType;
            m_bEnableRead = bRead;
            m_bEnableWrite = bWrite;
        }

        public string GetName() { return m_sName; }
        public string GetHashedPassword() { return m_sHashedPassword; }
        public UserTypes GetUserType() { return m_eUserType; }
        public bool IsReadEnabled() { return m_bEnableRead; }
        public bool IsWriteEnabled() { return m_bEnableWrite; }

        public User Clone()
        {
            return new User(m_sName, m_sHashedPassword, m_eUserType, m_bEnableRead, m_bEnableWrite);
        }
    }
}
