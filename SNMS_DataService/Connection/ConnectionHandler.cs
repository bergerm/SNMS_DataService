using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

using SNMS_DataService.Users;

namespace SNMS_DataService.Connection
{
    abstract class ConnectionHandler
    {
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        static void HandleServerConnection(TcpClient client, NetworkStream stream)
        {
            byte[] res = GetBytes("ok");
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
            byte[] res = GetBytes("ok");
            stream.Write(res, 0, res.Length);

            byte[] message = new byte[stream.Length];
            stream.ReadAsync(message, 0, (int)stream.Length);

            string sLoginMessage = GetString(message);

            string[] loginParameters = sLoginMessage.Split(',');
            if (loginParameters.Length != 2)
            {
                return;
            }

            string sUserName = loginParameters[0];
            string sPassword = loginParameters[1];

            UsersDictionary userDict = UsersDictionary.Instance();
            User user = userDict.GetUser(sUserName);

            if (user == null)
            {
                byte[] result = GetBytes("-1");
                stream.Write(res, 0, result.Length);
                return;
            }

            string sResponse = ((int)user.GetUserType()).ToString();
            byte[] responseBytes = GetBytes(sResponse);

            stream.Write(responseBytes, 0, responseBytes.Length);
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

            stream.ReadTimeout = 60000;

            try
            {
                byte[] message = new byte[stream.Length];
                stream.ReadAsync(message, 0, (int)stream.Length);

                string sConnectionTypeMessage = GetString(message);

                switch (sConnectionTypeMessage)
                {
                    case "server":
                        HandleServerConnection(tcpClient, stream);
                        break;

                    case "client":
                        HandleClientConnection(tcpClient, stream);
                        break;

                    default:
                        byte[] res = GetBytes("error");
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
