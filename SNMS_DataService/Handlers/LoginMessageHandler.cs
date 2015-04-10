using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

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
            if (user != null && sPassword == user.GetHashedPassword())
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
