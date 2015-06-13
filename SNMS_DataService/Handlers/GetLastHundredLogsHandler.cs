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
    class GetLastHunderdLogsHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_LAST_100_LOGS_ANSWER);

            string sComponentFilter = message.GetParameterAsString(0);
            string sUserNameFilter = message.GetParameterAsString(1);
            string sLogType = message.GetParameterAsString(2);
            string sMessage = message.GetParameterAsString(3);
            string sLink = message.GetParameterAsString(4);

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastLogsCount(sComponentFilter,
                                                                        sUserNameFilter,
                                                                        sLogType,
                                                                        sMessage,
                                                                        sLink));
            
            reader.Read();
            int dwNumOfUserLogs = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(dwNumOfUserLogs);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetLastLogs(sComponentFilter,
                                                                        sUserNameFilter,
                                                                        sLogType,
                                                                        sMessage,
                                                                        sLink));

            while (reader.Read())
            {
                string sLogID = reader["LogID"].ToString();
                responseMessage.AddParameter(sLogID);
                string sLogTime = reader["LogTime"].ToString();
                responseMessage.AddParameter(sLogTime);
                string sLogComponent = reader["LogComponent"].ToString();
                responseMessage.AddParameter(sLogComponent);
                string sLogUserName = reader["LogUserName"].ToString();
                responseMessage.AddParameter(sLogUserName);
                string sLogTypes = reader["LogType"].ToString();
                responseMessage.AddParameter(sLogTypes);
                string sLogMessage = reader["LogMessage"].ToString();
                responseMessage.AddParameter(sLogMessage);
                string sLogLink = reader["LogLink"].ToString();
                responseMessage.AddParameter(sLogLink);
            }

            reader.Close();

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
