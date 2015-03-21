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
        private static HandlerManager instance;

        Dictionary<ProtocolMessageType, Handler> m_clientHandlerDictionary;
        Dictionary<ProtocolMessageType, Handler> m_serverHandlerDictionary;
        NetworkStream m_networkStream;

        private HandlerManager()
        {
            m_clientHandlerDictionary = new Dictionary<ProtocolMessageType, Handler>();
            m_serverHandlerDictionary = new Dictionary<ProtocolMessageType, Handler>();
        }

        public static HandlerManager Instance()
        {
            if (instance == null)
            {
                instance = new HandlerManager();
            }
            return instance;
        }

        public void RegisterClientHandler(ProtocolMessageType type, Handler handler)
        {
            m_clientHandlerDictionary.Add(type, handler);
        }

        public void RegisterServerHandler(ProtocolMessageType type, Handler handler)
        {
            m_serverHandlerDictionary.Add(type, handler);
        }

        public bool HandleClientMessage(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessageType type = message.GetMessageType();

            if (!m_clientHandlerDictionary.Keys.Contains(type))
            {
                return false;
            }

            Handler handler = m_clientHandlerDictionary[type];

            return handler.Handle(message, stream);
        }

        public bool HandleServerMessage(ProtocolMessage message, NetworkStream stream)
        {
            ProtocolMessageType type = message.GetMessageType();

            if (!m_serverHandlerDictionary.Keys.Contains(type))
            {
                return false;
            }

            Handler handler = m_serverHandlerDictionary[type];

            return handler.Handle(message, stream);
        }
    }
}
