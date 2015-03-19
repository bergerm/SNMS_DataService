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
        virtual protected bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessage responseMessage = new ProtocolMessage();
            responseMessage.SetMessageType(ProtocolMessageType.PROTOCOL_MESSAGE_LOGIN_ANSWER);
            string sResponse;

            UsersDictionary usersDictionary = UsersDictionary.Instance();

            string sUserName = message.GetParameterAsString(0);
            string sPassword = message.GetParameterAsString(1);

            User user = usersDictionary.GetUser(sUserName);
            if (sPassword == user.GetHashedPassword())
            {
                sResponse = ProtocolMessage.PROTOCOL_CONSTANT_SUCCESS_MESSAGE;
            }
            else
            {
                sResponse = ProtocolMessage.PROTOCOL_CONSTANT_FAILURE_MESSAGE;
            }

            responseMessage.AddParameter(sResponse);

            UserTypes userType = user.GetUserType();
            byte[] userTypeBytes = BitConverter.GetBytes((int)userType);
            responseMessage.AddParameter(userTypeBytes, 4);

            byte[] response = Protocol.CraftMessage(responseMessage);
            stream.Write(response, 0, response.Length);

            return true;
        }
    }
}
