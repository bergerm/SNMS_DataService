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
using SNMS_DataService.Plugins;

namespace SNMS_DataService.Handlers
{
    class NewPluginHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            DatabaseGateway dbGateway = DatabaseGateway.Instance(null);

            string sFilePath = "C:\\pluginsFolder\\";
            string sFileName = message.GetParameterAsString(0);

            byte[] fileData = null ;
            message.GetParameter(ref fileData, 1);
            if (fileData.Length == 0)
            {
                return true;
            }

            sFileName += DateTime.Now.ToString("_MM_dd_yyyy_hh_mm_tt_fffffff");
            sFilePath += sFileName;
            //FileStream fileStream = File.Create(sFilePath);
            //fileStream.Close();

            //fileStream = new FileStream(sFilePath, FileMode.Append, FileAccess.Write);
            //StreamWriter writer = new StreamWriter(fileStream);
            //writer.Write(fileData);
            //writer.Close();
            //fileStream.Close();
            File.WriteAllBytes(sFilePath, fileData);

            string sErrorString = "";
            Plugin plugin = PluginParser.ParsePlugin(sFilePath, ref sErrorString);
            if (plugin == null)
            {
                ProtocolMessage errorMessage = new ProtocolMessage();
                errorMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_PLUGINS_LIST);

                errorMessage.AddParameter(0);
                errorMessage.AddParameter(sErrorString);
                return true;
            }

            string sFilePathForSQL = sFilePath.Replace('\\', '/');
            dbGateway.WriteQuery(QueryManager.NewPluginQuery(plugin.sName, plugin.sDescription, false, sFilePathForSQL));
            
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_PLUGINS_LIST);

            MySqlDataReader reader = dbGateway.ReadQuery(QueryManager.GetLastInsertID());
            if (reader == null)
            {
                return false;
            }
            reader.Read();
            int pluginID = Int32.Parse(reader[0].ToString());
            reader.Close();

            reader = dbGateway.ReadQuery(QueryManager.GetSpecificPluginQuery(pluginID));
            if (reader == null)
            {
                return false;
            }

            // contains 1 plugin
            responseMessage.AddParameter(1);

            reader.Read();
            int dwPluginId = Int32.Parse(reader["PluginID"].ToString());
            string sPluginName = reader["PluginName"].ToString();
            string sPluginDesc = reader["PluginDescription"].ToString();
            bool bPluginEnabled = (reader["PluginEnabled"].ToString() == "1")?true:false;
            reader.Close();

            //Parameter PluginID
            responseMessage.AddParameter(dwPluginId);
            //Parameter PluginName
            responseMessage.AddParameter(sPluginName);
            //Parameter PluginDescription
            responseMessage.AddParameter(sPluginDesc);
            //Parameter PluginEnabled
            responseMessage.AddParameter(bPluginEnabled);

            foreach (Variable variable in plugin.valiableList)
            {
                dbGateway.WriteQuery(QueryManager.NewPluginVariable(dwPluginId, variable.sName, variable.sType));
            }

            foreach (Sequence sequence in plugin.sequenceList)
            {
                dbGateway.WriteQuery(QueryManager.NewPluginSequence(dwPluginId, sequence.sName));
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
