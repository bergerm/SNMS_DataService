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
    class LogMessageHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            string sLogType = message.GetParameterAsString(0);
            string sLogMessage = message.GetParameterAsString(1);
            string sLogLink = message.GetParameterAsString(2);
            string sComponent = message.GetParameterAsString(3);
            string sUser = message.GetParameterAsString(4);

            dbGateway.WriteQuery(QueryManager.SaveLogMessage(sLogType, sLogMessage, sLogLink, sComponent, sUser));

            return true;
        }
    }
}
