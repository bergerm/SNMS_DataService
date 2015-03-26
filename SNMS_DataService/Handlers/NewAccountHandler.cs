using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using System.IO;

using SNMS_DataService.Database;
using SNMS_DataService.Connection;
using SNMS_DataService.Users;
using SNMS_DataService.Queries;
using MySql.Data.MySqlClient;

namespace SNMS_DataService.Handlers
{
    class NewAccountHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int pluginID = message.GetParameterAsInt(0);

            dbGateway.WriteQuery(QueryManager.NewAccountQuery(pluginID, "New Account", "", "", ""));
            
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_ACCOUNTS_LIST);

            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int accountID = Int32.Parse(reader[0].ToString());
            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificAccountQuery(accountID));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();

            int dwAccountId = Int32.Parse(reader["AccountID"].ToString());
            responseMessage.AddParameter(dwAccountId);
            string sAccountName = reader["AccountName"].ToString();
            responseMessage.AddParameter(sAccountName);
            string sAccountDesc = reader["AccountDescription"].ToString();
            responseMessage.AddParameter(sAccountDesc);
            string sAccountUserName = reader["AccountUserName"].ToString();
            responseMessage.AddParameter(sAccountUserName);
            string sAccountPassword = reader["AccountPassword"].ToString();
            responseMessage.AddParameter(sAccountPassword);  
                
            ConnectionHandler.SendMessage(stream, responseMessage);

            reader.Close();

            return true;
        }
    }
}
