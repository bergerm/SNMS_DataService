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
    class ConfigurationStatusMessageHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            int dwConfigurationID = message.GetParameterAsInt(0);
            string sStatus = message.GetParameterAsString(1);

            dbGateway.WriteQuery(QueryManager.SaveConfigurationStatusMessage(dwConfigurationID, sStatus));

            return true;
        }
    }
}
