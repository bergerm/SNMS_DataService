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
    class NewTriggerTypeHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int configurationID = message.GetParameterAsInt(0);

            dbGateway.WriteQuery(QueryManager.NewTriggeTypesQuery(configurationID, "New Trigger Type", ""));
            
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_TRIGGER_TYPES_LIST);

            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int triggerTypeID = Int32.Parse(reader[0].ToString());
            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificTriggerTypesQuery(triggerTypeID));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();

            int dwTriggerTypesID = Int32.Parse(reader["TriggerTypeID"].ToString());
            responseMessage.AddParameter(dwTriggerTypesID);
            string sTriggerTypeName = reader["TriggerTypeName"].ToString();
            responseMessage.AddParameter(sTriggerTypeName);
            string sTriggerTypeDesc = reader["TriggerTypeDescription"].ToString();
            responseMessage.AddParameter(sTriggerTypeDesc);

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            

            return true;
        }
    }
}
