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
    class ServerUpdateRequiredHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.ServerNeedsUpdateQuery());

            ProtocolMessage updateStatusMessage = new ProtocolMessage();
            updateStatusMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_SERVER_UPDATE_STATUS_ANSWER);

            reader.Read();
            if (reader["UpdateRequired"].ToString() == "1")
            {
                updateStatusMessage.AddParameter(true);
            }
            else
            {
                updateStatusMessage.AddParameter(false);
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, updateStatusMessage);
            return true;
        }
    }
}
