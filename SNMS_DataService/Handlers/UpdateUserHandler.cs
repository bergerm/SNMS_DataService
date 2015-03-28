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
    class UpdateUserHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int dwID = message.GetParameterAsInt(0);
            string sName = message.GetParameterAsString(1);
            string sPass = message.GetParameterAsString(2);
            int dwTypeID = message.GetParameterAsInt(3);
            bool bReadEnabled = message.GetParameterAsBool(4);
            bool bWriteEnabled = message.GetParameterAsBool(5);

            dbGateway.WriteQuery(QueryManager.UpdateUserQuery(dwID, sName, sPass, dwTypeID, bReadEnabled, bWriteEnabled));

            return true;
        }
    }
}
