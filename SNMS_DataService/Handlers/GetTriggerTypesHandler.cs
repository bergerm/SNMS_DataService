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
    class GetTriggerTypesHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_TRIGGER_TYPES_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            byte[] intBuffer = new byte[4];
            message.GetParameter(ref intBuffer, 0);
            int dwConfigurationID = BitConverter.ToInt32(intBuffer, 0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetTriggerTypesCountQuery(dwConfigurationID));
            
            // Parameter 1 - number of TriggerType
            reader.Read();
            int dwNumOfTriggerType = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfTriggerType), 4);

            reader = dbGateway.ReadQuery(QueryManager.GetTriggerTypesQuery(dwConfigurationID));

            while (reader.Read())
            {
                int dwTriggerTypeId = Int32.Parse(reader["TriggerTypeID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwTriggerTypeId), 4);
                string sTriggerTypeName = reader["TriggerTypeName"].ToString();
                responseMessage.AddParameter(sTriggerTypeName);
                string sTriggerTypeDescription = reader["TriggerTypeDescription"].ToString();
                responseMessage.AddParameter(sTriggerTypeDescription); 
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
