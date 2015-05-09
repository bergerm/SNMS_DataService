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
    class GetConfigurationStatusHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONFIGURATIONS_STATUS_LIST);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetConfigurationsStatus());

            List<int> configurationIDsList = new List<int>();
            List<string> configurationNamesList = new List<string>();
            List<string> statusesList = new List<string>();
            List<bool> expiredList = new List<bool>();

            while (reader.Read())
            {
                configurationIDsList.Add(Int32.Parse(reader["ConfigurationID"].ToString()));
                statusesList.Add(reader["Status"].ToString());
                expiredList.Add((reader["Expired"].ToString() == "1") ? true : false);
            }
            reader.Close();

            foreach (int id in configurationIDsList)
            {
                reader = dbGateway.ReadQuery(QueryManager.GetSpecificConfigurationQuery(id));
                reader.Read();
                configurationNamesList.Add(reader["ConfigurationName"].ToString());
                reader.Close();
            }

            int numOfStatuses = configurationIDsList.Count;
            responseMessage.AddParameter(numOfStatuses);

            for (int i = 0; i < numOfStatuses; i++)
            {
                responseMessage.AddParameter(configurationIDsList[i]);
                responseMessage.AddParameter(configurationNamesList[i]);
                responseMessage.AddParameter(statusesList[i]);
                responseMessage.AddParameter(expiredList[i]);
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
