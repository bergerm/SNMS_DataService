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
    class GetUserTypesHandler : Handler
    {
        virtual protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_USER_TYPES_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetUserTypesCountQuery());
            
            // Parameter 1 - number of UserTypes
            reader.Read();
            int dwNumOfUserTypes = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfUserTypes), 4);

            reader = dbGateway.ReadQuery(QueryManager.GetUserTypesQuery());

            while (reader.Read())
            {
                int dwUserTypeId = Int32.Parse(reader["UserTypeID"].ToString());
                responseMessage.AddParameter(BitConverter.GetBytes(dwUserTypeId), 4);
                string sUserTypeName = reader["UserTypeName"].ToString();
                responseMessage.AddParameter(sUserTypeName);        
            }

            byte[] response = Protocol.CraftMessage(responseMessage);
            stream.Write(response, 0, response.Length);

            return true;
        }
    }
}
