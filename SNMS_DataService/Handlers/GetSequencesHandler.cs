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
    class GetSequencesHandler
    {
        override bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_SEQUENCES_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            byte[] intBuffer = new byte[4];
            message.GetParameter(ref intBuffer, 0);
            int dwConfigurationID = BitConverter.ToInt32(intBuffer, 0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetSequencesCountQuery(dwConfigurationID));
            
            // Parameter 1 - number of Sequences
            reader.Read();
            int dwNumOfSequences = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfSequences), 4);

            reader = dbGateway.ReadQuery(QueryManager.GetSequencesQuery(dwConfigurationID));

            while (reader.Read())
            {
                int dwSequenceId = Int32.Parse(reader["SequenceID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwSequenceId), 4);
                string sSequenceName = reader["SequenceName"].ToString();
                responseMessage.AddParameter(sSequenceName);
                int dwSequenceEnabled = byte.Parse(reader["ConfigurationEnabled"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwSequenceEnabled), 1);         
            }

            byte[] response = Protocol.CraftMessage(responseMessage);
            stream.Write(response, 0, response.Length);

            return true;
        }
    }
}
