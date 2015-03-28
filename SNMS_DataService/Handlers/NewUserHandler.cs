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
    class NewUserHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            dbGateway.WriteQuery(QueryManager.NewUserQuery("New User", "", 1, false, false));
            
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_USERS_LIST);

            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int userID = Int32.Parse(reader[0].ToString());
            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificUserQuery(userID));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();

            int dwUserId = Int32.Parse(reader["UserID"].ToString());
            responseMessage.AddParameter(dwUserId);
            string sUserName = reader["UserName"].ToString();
            responseMessage.AddParameter(sUserName);
            string sUserHashedPassword = reader["UserHashedPassword"].ToString();
            responseMessage.AddParameter(sUserHashedPassword);
            int dwUserTypeId = Int32.Parse(reader["UserTypeID"].ToString());
            responseMessage.AddParameter(dwUserTypeId);
            int dwReadEnabled = byte.Parse(reader["UserEnableRead"].ToString());
            responseMessage.AddParameter((dwReadEnabled != 0) ? true : false);
            int dwWriteEnabled = byte.Parse(reader["UserEnableWrite"].ToString());
            responseMessage.AddParameter((dwWriteEnabled != 0) ? true : false);  
                
            ConnectionHandler.SendMessage(stream, responseMessage);

            reader.Close();

            return true;
        }
    }
}
