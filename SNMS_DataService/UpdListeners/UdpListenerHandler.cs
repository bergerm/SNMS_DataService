using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SNMS_DataService.UpdListeners
{
    class UdpListenerHandler
    {
        static UdpListenerHandler m_instance;
        
        List<IPAddress> m_listOfAddresses;
        Mutex m_mutex;

        Timer m_wipeTimer;
        const int WIPE_TIME = 300;

        const int UDP_PORT = 8454;

        static public UdpListenerHandler Instance()
        {
            if (m_instance == null)
            {
                m_instance = new UdpListenerHandler();
            }

            return m_instance;
        }

        protected UdpListenerHandler()
        {
            m_listOfAddresses = new List<IPAddress>();
            m_mutex = new Mutex();
            m_wipeTimer = null;
        }

        public void RegisterListener(string sIpAddress)
        {
            IPAddress ip = IPAddress.Parse(sIpAddress);
            if (m_listOfAddresses.Contains(ip))
            {
                return;
            }
            m_mutex.WaitOne();
            m_listOfAddresses.Add(ip);
            m_mutex.ReleaseMutex();
        }

        void Wipe()
        {
            m_mutex.WaitOne();
            m_listOfAddresses.Clear();
            m_mutex.ReleaseMutex();
        }

        public void StartTimer()
        {
            if (m_wipeTimer != null)
            {
                StopTimer();
            }
            
            m_wipeTimer = new Timer(e => Wipe(),
                                        null,
                                        TimeSpan.Zero,
                                        TimeSpan.FromMinutes(WIPE_TIME));

        }

        public void StopTimer()
        {
            if (m_wipeTimer == null)
            {
                m_wipeTimer.Dispose();
            }
        }

        public void SendMessage(byte[] message)
        {
            foreach (IPAddress address in m_listOfAddresses)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint endPoint = new IPEndPoint(address, UDP_PORT);

                sock.SendTo(message, endPoint);

                sock.Close();
            }
        }
    }
}
