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
    class NewTriggerHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int configurationID = message.GetParameterAsInt(0);
            int triggerTypeID = message.GetParameterAsInt(1);

            dbGateway.WriteQuery(QueryManager.NewTriggerQuery(configurationID, triggerTypeID, "New Trigger", "", "", -1, "", false));
            
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_TRIGGERS_LIST);

            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int triggerID = Int32.Parse(reader[0].ToString());
            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificTriggerQuery(triggerID));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();

            int dwTriggerID = Int32.Parse(reader["TriggerID"].ToString());
            responseMessage.AddParameter(dwTriggerID);
            //int dwConfigurationID = Int32.Parse(reader["ConfigurationID"].ToString());
            //responseMessage.AddParameter(dwTriggerID);
            //int dwTriggerTypeID = Int32.Parse(reader["TriggerTypeID"].ToString());
            //responseMessage.AddParameter(dwTriggerID);
            string sTriggerName = reader["TriggerName"].ToString();
            responseMessage.AddParameter(sTriggerName);
            string sTriggerDesc = reader["TriggerDescription"].ToString();
            responseMessage.AddParameter(sTriggerDesc);
            string sTriggerValue = reader["TriggerValue"].ToString();
            responseMessage.AddParameter(sTriggerValue);
            int dwReactionID = Int32.Parse(reader["ReactionSequenceID"].ToString());
            responseMessage.AddParameter(dwReactionID);
            string sReactionsValue = reader["ReactionValue"].ToString();
            responseMessage.AddParameter(sTriggerValue);
            int dwTriggerEnabled = byte.Parse(reader["TriggerEnabled"].ToString());
            responseMessage.AddParameter((dwTriggerEnabled != 0) ? true : false); 

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            

            return true;
        }
    }
}
