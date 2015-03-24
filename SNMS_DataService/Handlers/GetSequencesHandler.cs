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
    class GetSequencesHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_SEQUENCES_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            int dwConfigurationID = message.GetParameterAsInt(0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetSequencesCountQuery(dwConfigurationID));
            
            // Parameter 1 - number of Sequences
            reader.Read();
            int dwNumOfSequences = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfSequences), 4);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSequencesQuery(dwConfigurationID));

            while (reader.Read())
            {
                int dwSequenceId = Int32.Parse(reader["SequenceID"].ToString());
                responseMessage.AddParameter(dwSequenceId);
                string sSequenceName = reader["SequenceName"].ToString();
                responseMessage.AddParameter(sSequenceName);
                int dwSequenceEnabled = byte.Parse(reader["SequenceEnabled"].ToString());
                responseMessage.AddParameter((dwSequenceEnabled != 0) ? true : false);         
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
