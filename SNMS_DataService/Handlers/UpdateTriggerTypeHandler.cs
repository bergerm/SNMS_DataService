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
    class UpdateTriggerTypeHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int dwID = message.GetParameterAsInt(0);
            int configurationID = message.GetParameterAsInt(1);
            string sName = message.GetParameterAsString(2);
            string sDesc = message.GetParameterAsString(3);

            dbGateway.WriteQuery(QueryManager.UpdateTriggeTypesQuery(dwID, configurationID, sName, sDesc));

            return true;
        }
    }
}
