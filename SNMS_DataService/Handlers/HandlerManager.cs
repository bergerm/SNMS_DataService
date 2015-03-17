using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using SNMS_DataService.Connection;
using SNMS_DataService.Database;

namespace SNMS_DataService.Handlers
{
    class HandlerManager
    {
        Dictionary<ProtocolMessageType, Handler> m_handlerDictionary;
        NetworkStream m_networkStream;

        public HandlerManager()
        {
            m_handlerDictionary = new Dictionary<ProtocolMessageType,Handler>();
        }

        public void RegisterHandler(ProtocolMessageType type, Handler handler)
        {
            m_handlerDictionary.Add(type, handler);
        }

        public bool HandleMessage(ProtocolMessage message)
        {
            ProtocolMessageType type = message.GetMessageType();

            if (!m_handlerDictionary.Keys.Contains(type))
            {
                return false;
            }

            Handler handler = m_handlerDictionary[type];

            return handler.Handle(message);
        }
    }
}
