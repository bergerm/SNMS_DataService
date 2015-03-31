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
    class GetConfigurationVariablesHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_VARIABLES_LIST);

            int dwConfigurationID = message.GetParameterAsInt(0);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetVariablesCountQuery(dwConfigurationID));
            
            // Parameter 1 - number of Users
            reader.Read();
            int dwNumOfVariables = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(dwNumOfVariables);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetVariablesQuery(dwConfigurationID));

            while (reader.Read())
            {
                int dwUserId = Int32.Parse(reader["ConfigurationVariableID"].ToString());
                responseMessage.AddParameter(dwUserId);
                string sVariableName = reader["VariableName"].ToString();
                responseMessage.AddParameter(sVariableName);
                string sVariableType = reader["VariableType"].ToString();
                responseMessage.AddParameter(sVariableType);
                string sVariableValue = reader["VariableValue"].ToString();
                responseMessage.AddParameter(sVariableValue);
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
