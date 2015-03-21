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
    class GetConfigurationsHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONFIGURATIONS_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            byte[] intBuffer = new byte[4];
            message.GetParameter(ref intBuffer, 0);
            int dwAccountID = BitConverter.ToInt32(intBuffer, 0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetConfigurationCountQuery(dwAccountID));
            
            // Parameter 1 - number of Configurations
            reader.Read();
            int dwNumOfPlugins = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfPlugins),4);

            reader = dbGateway.ReadQuery(QueryManager.GetConfigurationQuery(dwAccountID));

            while (reader.Read())
            {
                int dwConfigurationId = Int32.Parse(reader["ConfigurationID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwConfigurationId), 4);
                string sConfigurationName = reader["ConfigurationName"].ToString();
                responseMessage.AddParameter(sConfigurationName);
                string sConfigurationDesc = reader["ConfigurationDescription"].ToString();
                responseMessage.AddParameter(sConfigurationDesc);
                int dwConfigurationEnabled = byte.Parse(reader["ConfigurationEnabled"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwConfigurationId), 1);         
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
