using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

using SNMS_DataService.Users;
using SNMS_DataService.Handlers;
using SNMS_DataService.UpdListeners;

namespace SNMS_DataService.Connection
{
    abstract class ConnectionHandler
    {
        private const int CONNECTION_TIMEOUT = 300000;

        public static ProtocolMessage GetMessage(Stream stream)
        {
            try
            {
                stream.ReadTimeout = CONNECTION_TIMEOUT;
                int bufferSize = 2048;
                byte[] resp = new byte[bufferSize];
                int bytesread = stream.Read(resp, 0, 4);
                int messageLength = BitConverter.ToInt32(resp, 0);
                int totalRead = 0;

                var memStream = new MemoryStream();
                bytesread = 0;
                while (totalRead < messageLength)
                {
                    int bytesToRead = Math.Min(resp.Length, messageLength - totalRead);
                    bytesread = stream.Read(resp, 0, bytesToRead);
                    totalRead += bytesread;
                    memStream.Write(resp, 0, bytesread);
                }

                ProtocolMessage message = Protocol.ParseMessage(memStream.ToArray());
                return message;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void SendMessage(Stream stream, ProtocolMessage message)
        {
            byte[] response = Protocol.CraftMessage(message);
            byte[] responseSize = BitConverter.GetBytes(response.Length);
            // Send message size
            stream.Write(responseSize, 0, 4);
            // Send message
            stream.Write(response, 0, response.Length);
            stream.Flush();
        }
        
        static void HandleServerConnection(TcpClient server, NetworkStream stream)
        {
            ProtocolMessage connectionOkMessage = new ProtocolMessage();
            connectionOkMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION_RESPONSE);
            SendMessage(stream, connectionOkMessage);

            HandlerManager handlerManager = HandlerManager.Instance();

            UdpListenerHandler udpListenersHandler = UdpListenerHandler.Instance();

            ProtocolMessage message = GetMessage(stream);
            while (message != null)
            {
                // resend the message to all listeners
                udpListenersHandler.SendMessage(Protocol.CraftMessage(message));

                if (!handlerManager.HandleServerMessage(message, stream))
                {
                    return;
                }
                message = GetMessage(stream);
            }
        }

        static void HandleClientConnection(TcpClient client, NetworkStream stream)
        {
            ProtocolMessage connectionOkMessage = new ProtocolMessage();
            connectionOkMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION_RESPONSE);
            SendMessage(stream, connectionOkMessage);

            HandlerManager handlerManager = HandlerManager.Instance();

            ProtocolMessage message = GetMessage(stream);
            while (message != null)
            {
                if (!handlerManager.HandleClientMessage(message, stream))
                {
                    return;
                }
                message = GetMessage(stream);
            }
        }

        static void HandleUdpListenerConnection(TcpClient client, NetworkStream stream, string ipAddress)
        {
            ProtocolMessage connectionOkMessage = new ProtocolMessage();
            connectionOkMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION_RESPONSE);
            SendMessage(stream, connectionOkMessage);

            UdpListenerHandler udpListenerHandler = UdpListenerHandler.Instance();
            udpListenerHandler.RegisterListener(ipAddress);
        }

        public static void HandleConnection(object client)
        {
            if (client == null)
            {
                return;
            }

            TcpClient tcpClient = (TcpClient)client;

            NetworkStream stream = tcpClient.GetStream();

            if (!stream.CanRead)
            {
                stream.Close();
                tcpClient.Close();
                return;
            }

            try
            {
                ProtocolMessage connectionMessage = GetMessage(stream);
                if(connectionMessage == null || connectionMessage.GetMessageType() != ProtocolMessageType.PROTOCOL_MESSAGE_CONNECTION)
                {
                    return;
                }
                string sConnectionTypeMessage = connectionMessage.GetParameterAsString(0);

                switch (sConnectionTypeMessage)
                {
                    case "server":
                        HandleServerConnection(tcpClient, stream);
                        break;

                    case "client":
                        HandleClientConnection(tcpClient, stream);
                        break;

                    case "udpListener":
                        string sAddress = connectionMessage.GetParameterAsString(1);
                        HandleUdpListenerConnection(tcpClient, stream, sAddress);
                        break;

                    default:
                        byte[] res = ProtocolMessage.GetBytes("error");
                        stream.Write(res, 0, res.Length);
                        return;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                stream.Close();
                tcpClient.Close();
            }
        }
    }
}
