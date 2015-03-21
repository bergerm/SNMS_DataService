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
    class GetPluginsHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_PLUGINS_LIST);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);
            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetPluginsCountQuery());
            if (reader == null)
            {
                return false;
            }
            
            // Parameter 1 - number of plugins
            reader.Read();
            int dwNumOfPlugins = Int32.Parse(reader[0].ToString());
            responseMessage.AddParameter(BitConverter.GetBytes(dwNumOfPlugins),4);

            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetPluginsQuery());
            if (reader == null)
            {
                return false;
            }

            while (reader.Read())
            {
                int dwPluginId = Int32.Parse(reader["PluginID"].ToString());
                string sPluginName = reader["PluginName"].ToString();
                string sPluginDesc = reader["PluginDescription"].ToString();
                int dwPluginEnabled = Int32.Parse(reader["PluginEnabled"].ToString());
                int dwBlobColumnIndex = 4;
 
                MemoryStream memStream = new MemoryStream();

                int bufferSize = 1024;
                byte[] blobBuffer = new byte[bufferSize];
                // Reset the starting byte for the new BLOB.
                long startIndex = 0;
                // Read bytes into outByte[] and retain the number of bytes returned.
                long retval = reader.GetBytes(dwBlobColumnIndex, startIndex, blobBuffer, 0, bufferSize);

                // Continue while there are bytes beyond the size of the buffer.
                while (retval == bufferSize)
                {
                    memStream.Write(blobBuffer,0,bufferSize);
                    memStream.Flush();

                    // Reposition start index to end of last buffer and fill buffer.
                    startIndex += bufferSize;
                    retval = reader.GetBytes(dwBlobColumnIndex, startIndex, blobBuffer, 0, bufferSize);
                }

                // Write the remaining buffer.
                memStream.Write(blobBuffer, 0, bufferSize);
                memStream.Flush();

                //Parameter PluginID
                responseMessage.AddParameter(BitConverter.GetBytes(dwPluginId), 4);
                //Parameter PluginName
                responseMessage.AddParameter(sPluginName);
                //Parameter PluginDescription
                responseMessage.AddParameter(sPluginDesc);
                //Parameter PluginEnabled
                responseMessage.AddParameter(BitConverter.GetBytes(dwPluginEnabled), 4);
                //Parameter PluginBLOB
                responseMessage.AddParameter(memStream.GetBuffer(), (int)memStream.Length); 
                
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            reader.Close();

            return true;
        }
    }
}
