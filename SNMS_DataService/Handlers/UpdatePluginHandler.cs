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
    class UpdatePluginHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int dwID = message.GetParameterAsInt(0);
            string sName = message.GetParameterAsString(1);
            string sDesc = message.GetParameterAsString(2);
            bool bEnabled = message.GetParameterAsBool(3);

            dbGateway.WriteQuery(QueryManager.UpdatePluginQuery(dwID, sName, sDesc, bEnabled));

            dbGateway.WriteQuery(QueryManager.NewDataAvailableQuery());

            return true;
        }
    }
}
