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
    class GetAccountsHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_ACCOUNTS_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            byte[] intBuffer = new byte[4];
            message.GetParameter(ref intBuffer, 0);
            int dwPluginID = BitConverter.ToInt32(intBuffer, 0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetAccountsCountQuery(dwPluginID));
            
            // Parameter 1 - number of accpimts
            reader.Read();
            int dwNumOfPlugins = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfPlugins),4);

            reader = dbGateway.ReadQuery(QueryManager.GetAccountsQuery(dwPluginID));

            while (reader.Read())
            {
                int dwAccountId = Int32.Parse(reader["AccountID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwAccountId), 4);
                string sAccountName = reader["AccountName"].ToString();
                responseMessage.AddParameter(sAccountName);
                string sAccountDesc = reader["AccountDescription"].ToString();
                responseMessage.AddParameter(sAccountDesc);
                string sAccountUserName = reader["AccountUserName"].ToString();
                responseMessage.AddParameter(sAccountUserName);
                string sAccountPassword = reader["AccountPassword"].ToString();
                responseMessage.AddParameter(sAccountPassword);                
            }

            byte[] response = Protocol.CraftMessage(responseMessage);
            stream.Write(response, 0, response.Length);

            return true;
        }
    }
}
