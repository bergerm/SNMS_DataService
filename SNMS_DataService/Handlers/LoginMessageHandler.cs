using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;

using SNMS_DataService.Connection;
using SNMS_DataService.Users;

namespace SNMS_DataService.Handlers
{
    class LoginMessageHandler : Handler
    {
        override protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_LOGIN_ANSWER);

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            string sUserName = message.GetParameterAsString(0);
            string sPassword = message.GetParameterAsString(1);

            usersDictionary.LoadUsers();

            User user = usersDictionary.GetUser(sUserName);
            //if (user != null && sPassword == user.GetHashedPassword())
            bool bSucced = false;
            if (user != null)
            {
                MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(sPassword);
                byte[] hash = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                string sHashedPassword = sb.ToString();

                if (sHashedPassword == user.GetHashedPassword())
                {
                    bSucced = true;
                }
            }

            if (bSucced)
            {
                responseMessage.AddParameter(ProtocolMessage.PROTOCOL_CONSTANT_SUCCESS_MESSAGE);
                responseMessage.AddParameter((int)user.GetUserType());
            }
            else
            {
                responseMessage.AddParameter(ProtocolMessage.PROTOCOL_CONSTANT_FAILURE_MESSAGE);
            }

            ConnectionHandler.SendMessage(stream, responseMessage);

            return true;
        }
    }
}
