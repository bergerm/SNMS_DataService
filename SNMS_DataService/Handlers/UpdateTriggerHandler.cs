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
    class UpdateTriggerHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            int dwID = message.GetParameterAsInt(0);
            int dwConfigurationID = message.GetParameterAsInt(1);
            int dwTriggerTypeID = message.GetParameterAsInt(2);
            string sName = message.GetParameterAsString(3);
            string sDesc = message.GetParameterAsString(4);
            string sValue = message.GetParameterAsString(5);
            int dwReactionSequenceID = message.GetParameterAsInt(6);
            string sReactionValue = message.GetParameterAsString(7);
            bool bEnabled = message.GetParameterAsBool(8);

            dbGateway.WriteQuery(QueryManager.UpdateTriggerQuery(dwID, dwConfigurationID, dwTriggerTypeID, sName, sDesc, sValue, dwReactionSequenceID, sReactionValue, bEnabled));

            dbGateway.WriteQuery(QueryManager.NewDataAvailableQuery());

            return true;
        }
    }
}
