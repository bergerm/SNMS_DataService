using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using SNMS_DataService.Connection;

namespace SNMS_DataService.Handlers
{
    abstract class Handler
    {
        abstract bool HandlerLogic(ProtocolMessage message, NetworkStream stream)
        {
            return true;
        }

        public bool Handle(ProtocolMessage message, NetworkStream stream)
        {
            return HandlerLogic(message, stream);
        }
    }
}
