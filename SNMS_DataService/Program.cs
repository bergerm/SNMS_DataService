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
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_PLUGINS, new GetPluginsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_ACCOUNTS, new GetAccountsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_CONFIGURATIONS, new GetConfigurationsHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_SEQUENCES, new GetSequencesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGER_TYPES, new GetTriggerTypesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_TRIGGERS, new GetTriggersHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USER_TYPES, new GetUserTypesHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_GET_USERS, new GetUsersHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_ACCOUNT, new NewAccountHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_ACCOUNT, new UpdateAccountHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_NEW_CONFIGURATION, new NewConfigurationHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_CONFIGURATION, new UpdateConfigurationHandler());
            manager.RegisterClientHandler(ProtocolMessageType.PROTOCOL_MESSAGE_UPDATE_SEQUENCE, new UpdateSequenceHandler());
        }

        static void Main(string[] args)
        {
            DatabaseGateway.DatabaseParameters parameters = new DatabaseGateway.DatabaseParameters(DB_HOST, DB_USER, DB_PW, DB_NAME);
            DatabaseGateway dbGate = DatabaseGateway.Instance(parameters);
            //QueryManager queryManager = new QueryManager();

            HandlerManager handlerManager = HandlerManager.Instance();
            PopulateClientHandlerManager(handlerManager);

            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(localAddr,TCP_PORT);

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
