﻿using System;
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
    class GetUsersHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_USERS_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetUsersCountQuery());
            
            // Parameter 1 - number of Users
            reader.Read();
            int dwNumOfUsers = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(dwNumOfUsers);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetUsersQuery());

            while (reader.Read())
            {
                int dwUserId = Int32.Parse(reader["UserID"].ToString());
                responseMessage.AddParameter(dwUserId);
                string sUserName = reader["UserName"].ToString();
                responseMessage.AddParameter(sUserName);
                string sHashedPassword = reader["UserHashedPassword"].ToString();
                responseMessage.AddParameter(sHashedPassword);
                int dwUserTypeId = Int32.Parse(reader["UserTypeID"].ToString());
                responseMessage.AddParameter(dwUserTypeId);
                byte userEnableRead = byte.Parse(reader["UserEnableRead"].ToString());
                responseMessage.AddParameter((userEnableRead==1)?true:false);
                int userEnableWrite = byte.Parse(reader["UserEnableWrite"].ToString());
                responseMessage.AddParameter((userEnableWrite == 1) ? true : false);
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
