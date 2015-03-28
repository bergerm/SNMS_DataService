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
    class UpdateSequenceHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int dwID = message.GetParameterAsInt(0);
            int configurationID = message.GetParameterAsInt(1);
            bool bEnabled = message.GetParameterAsBool(2);

            dbGateway.WriteQuery(QueryManager.UpdateConfigurationSequenceQuery(dwID, configurationID, bEnabled));

            return true;
        }
    }
}
