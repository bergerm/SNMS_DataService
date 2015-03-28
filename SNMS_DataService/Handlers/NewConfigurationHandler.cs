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
    class NewConfigurationHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int accountID = message.GetParameterAsInt(0);

            // HERE WE SHOULD CHECK WHAT SEQUENCES ARE NEEDED FOR THIS CONFIGURATION
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetSpecificAccountQuery(accountID));
            reader.Read();
            int dwPluginId = Int32.Parse(reader["PluginID"].ToString());
            reader.Close();

            dbGateway.WriteQuery(QueryManager.NewConfigurationQuery(accountID, "New Configuration", "", false));

            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONFIGURATIONS_LIST);

            reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int configurationId = Int32.Parse(reader[0].ToString());
            reader.Close();

            // HERE WE SHOULD ADD ALL SEQUENCES TO THE NEW CREATED CONFIGURATION
            reader = dbGateway.ReadQuery(QueryManager.GetSequencesForPluginQuery(dwPluginId));
            List<int> sequenceIdList = new List<int>();
            while (reader.Read())
            {
                sequenceIdList.Add(Int32.Parse(reader["SequenceID"].ToString()));
            }
            reader.Close();
            foreach (int id in sequenceIdList)
            {
                int sequenceID = id;
                dbGateway.WriteQuery(QueryManager.NewConfigurationSequenceQuery(configurationId, sequenceID, true));
            }

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificConfigurationQuery(configurationId));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();

            int dwConfigurationId = Int32.Parse(reader["ConfigurationID"].ToString());
            responseMessage.AddParameter(dwConfigurationId);
            string sConfigurationName = reader["ConfigurationName"].ToString();
            responseMessage.AddParameter(sConfigurationName);
            string sConfigurationDesc = reader["ConfigurationDescription"].ToString();
            responseMessage.AddParameter(sConfigurationDesc);
            int dwConfigurationEnabled = byte.Parse(reader["ConfigurationEnabled"].ToString());
            responseMessage.AddParameter((dwConfigurationEnabled != 0) ? true : false);  
                
            ConnectionHandler.SendMessage(stream, responseMessage);

            reader.Close();

            return true;
        }
    }
}
