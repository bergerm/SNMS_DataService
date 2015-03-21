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
                byte[] resp = new byte[2048];
                int bytesread = stream.Read(resp, 0, 4);
                int messageLength = BitConverter.ToInt32(resp, 0);

                var memStream = new MemoryStream();
                bytesread = 0;
                while (bytesread < messageLength)
                {
                    int bytesToRead = Math.Min(resp.Length, messageLength - bytesread);
                    bytesread = stream.Read(resp, 0, bytesToRead);
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
        
        static void HandleServerConnection(TcpClient client, NetworkStream stream)
        {
            byte[] res = ProtocolMessage.GetBytes("ok");
            stream.Write(res, 0, res.Length);

            int dwReadBytes = 0;
            while (dwReadBytes > 0)
            {
               /* int dwMessageSize = 0;
                int dwChunks = 0;
                int dwRemainder = 0;
                byte[] messageSizeBuffer = new byte[4];
                byte[] buffer = new byte[1024];
                stream.Read(messageSizeBuffer, 0, 4);

                // If the system architecture is little-endian reverse the byte array. 
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(messageSizeBuffer);
                }
                dwMessageSize = BitConverter.ToInt32(messageSizeBuffer, 0);
                dwChunks = (int)Math.Floor((double)dwMessageSize / 1024);
                dwRemainder = dwMessageSize - dwChunks * 1024;

                byte[] readMessageBytes = new byte[dwMessageSize];*/
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream); 
                

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
