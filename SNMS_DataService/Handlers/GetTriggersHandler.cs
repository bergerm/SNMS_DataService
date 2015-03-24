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
    class GetTriggersHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_TRIGGERS_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            int dwConfigurationID = message.GetParameterAsInt(0);

            int dwTriggerTypeID = message.GetParameterAsInt(1);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetTriggersCountQuery(dwConfigurationID, dwTriggerTypeID));
            
            // Parameter 1 - number of Trigger
            reader.Read();
            int dwNumOfTrigger = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfTrigger), 4);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetTriggersQuery(dwConfigurationID, dwTriggerTypeID));

            while (reader.Read())
            {
                int dwTriggerId = Int32.Parse(reader["TriggerID"].ToString());
                responseMessage.AddParameter(dwTriggerId);
                string sTriggerName = reader["TriggerName"].ToString();
                responseMessage.AddParameter(sTriggerName);
                string sTriggerDescription = reader["TriggerDescription"].ToString();
                responseMessage.AddParameter(sTriggerDescription);
                string sTriggerValue = reader["TriggerValue"].ToString();
                responseMessage.AddParameter(sTriggerValue);
                int dwReactionSequenceId = Int32.Parse(reader["ReactionSequenceID"].ToString());
                responseMessage.AddParameter(dwReactionSequenceId);
                string sReactionValue = reader["ReactionValue"].ToString();
                responseMessage.AddParameter(sReactionValue);
                int dwTriggerEnabled = byte.Parse(reader["TriggerEnabled"].ToString());
                responseMessage.AddParameter((dwTriggerEnabled != 0)?1:0);  
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
