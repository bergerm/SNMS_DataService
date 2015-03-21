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

            byte[] intBuffer = new byte[4];
            message.GetParameter(ref intBuffer, 0);
            int dwConfigurationID = BitConverter.ToInt32(intBuffer, 0);

            message.GetParameter(ref intBuffer, 1);
            int dwTriggerTypeID = BitConverter.ToInt32(intBuffer, 0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetTriggersCountQuery(dwConfigurationID, dwTriggerTypeID));
            
            // Parameter 1 - number of Trigger
            reader.Read();
            int dwNumOfTrigger = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfTrigger), 4);

            reader = dbGateway.ReadQuery(QueryManager.GetTriggersQuery(dwConfigurationID, dwTriggerTypeID));

            while (reader.Read())
            {
                int dwTriggerId = Int32.Parse(reader["TriggerID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwTriggerId), 4);
                string sTriggerName = reader["TriggerName"].ToString();
                responseMessage.AddParameter(sTriggerName);
                string sTriggerDescription = reader["TriggerDescription"].ToString();
                responseMessage.AddParameter(sTriggerDescription);
                string sTriggerValue = reader["TriggerValue"].ToString();
                responseMessage.AddParameter(sTriggerValue);
                int dwReactionSequenceId = Int32.Parse(reader["ReactionSequenceID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwReactionSequenceId), 4);
                string sReactionValue = reader["ReactionValue"].ToString();
                responseMessage.AddParameter(sReactionValue);
                int dwTriggerEnabled = byte.Parse(reader["TriggerEnabled"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwTriggerEnabled), 1);  
            }

            byte[] response = Protocol.CraftMessage(responseMessage);
            stream.Write(response, 0, response.Length);

            return true;
        }
    }
}
