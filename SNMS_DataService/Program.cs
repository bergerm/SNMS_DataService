using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using SNMS_DataService.Database;
using SNMS_DataService.Queries;
using SNMS_DataService.Connection;
using SNMS_DataService.Handlers;
using SNMS_DataService.UpdListeners;

namespace SNMS_DataService
{
    class Program
    {
        const int TCP_PORT = 56824;
        const string DB_HOST = "localhost";
        const string DB_USER = "root";
        const string DB_PW = "";
        const string DB_NAME = "snms_db";

        static void PopulateClientHandlerManager(HandlerManager manager)
        {
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_LOGIN_REQUEST, new LoginMessageHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS, new GetPluginsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_PLUGIN, new NewPluginHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_PLUGIN, new UpdatePluginHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_PLUGIN, new DeletePluginHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_ACCOUNTS, new GetAccountsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_ACCOUNT, new NewAccountHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_ACCOUNT, new UpdateAccountHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_ACCOUNT, new DeleteAccountHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS, new GetConfigurationsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_CONFIGURATION, new NewConfigurationHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_CONFIGURATION, new UpdateConfigurationHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_CONFIGURATION, new DeleteConfigurationHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_VARIABLES, new GetConfigurationVariablesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_VARIABLES, new UpdateConfigurationVariableHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_SEQUENCES, new GetSequencesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_SEQUENCE, new UpdateSequenceHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_SEQUENCE, new DeleteSequenceHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGER_TYPES, new GetTriggerTypesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_TRIGGER_TYPE, new NewTriggerTypeHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_TRIGGER_TYPE, new UpdateTriggerTypeHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_TRIGGER_TYPE, new DeleteTriggerTypeHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGERS, new GetTriggersHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_TRIGGER, new NewTriggerHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_TRIGGER, new UpdateTriggerHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_TRIGGER, new DeleteTriggerHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USER_TYPES, new GetUserTypesHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USERS, new GetUsersHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_USER, new NewUserHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_USER, new UpdateUserHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_DELETE_USER, new DeleteUserHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_SAVE_LOG_MESSAGE, new LogMessageHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_LAST_100_LOGS, new GetLastHunderdLogsHandler());

            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS_STATUS, new GetConfigurationStatusHandler());
        }

        static void PopulateServerHandlerManager(HandlerManager manager)
        {
            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS, new GetPluginsHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS_WITH_BLOBS, new GetPluginsWithBlobHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_ACCOUNTS, new GetAccountsHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS, new GetConfigurationsHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_VARIABLES, new GetConfigurationVariablesHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_SEQUENCES, new GetSequencesHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGER_TYPES, new GetTriggerTypesHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGERS, new GetTriggersHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USER_TYPES, new GetUserTypesHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USERS, new GetUsersHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_SERVER_UPDATE_STATUS, new ServerUpdateRequiredHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_SERVER_UPDATED, new ServerUpdatedHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_SAVE_LOG_MESSAGE, new LogMessageHandler());

            manager.RegisterServerHandler(ProtocolMessageType.PROTOCOL_MESSAGE_SAVE_CONFIGURATION_STATUS_MESSAGE, new ConfigurationStatusMessageHandler());

        }

        static void Main(string[] args)
        {
            DatabaseGateway.DatabaseParameters parameters = new DatabaseGateway.DatabaseParameters(DB_HOST, DB_USER, DB_PW, DB_NAME);
            DatabaseGateway dbGate = DatabaseGateway.Instance(parameters);
            //QueryManager queryManager = new QueryManager();

            HandlerManager handlerManager = HandlerManager.Instance();
            PopulateClientHandlerManager(handlerManager);
            PopulateServerHandlerManager(handlerManager);

            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localAddr,TCP_PORT);

            // used to handle UdpListeners (output API)
            UdpListenerHandler udpListenerHandler = UdpListenerHandler.Instance();

            try
            {
                listener.Start();

                while(true) 
                {
                    TcpClient client = listener.AcceptTcpClient();

                    ParameterizedThreadStart parameterizedThread = new ParameterizedThreadStart(ConnectionHandler.HandleConnection);
                    Thread thread = new Thread(parameterizedThread);
                    thread.Start(client);
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                listener.Stop();
            }

        }
    }
}
