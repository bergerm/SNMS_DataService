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
    class DeleteConfigurationHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            UsersDictionary usersDictionary = UsersDictionary.Instance();

            int dwConfigurationID = message.GetParameterAsInt(0);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            dbGateway.WriteQuery(QueryManager.DeleteConfigurationQuery(dwConfigurationID));

            dbGateway.WriteQuery(QueryManager.NewDataAvailableQuery());

            return true;
        }
    }
}
