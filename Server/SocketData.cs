using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SocketData
    {
        public int ReceivedBytes { get; set; }
        public Socket Socket { get; private set; }
        public byte[] Buffer { get; private set; }

        public SocketData(Socket socket, int size)
        {
            Socket = socket;
            Buffer = new byte[size];
        }

        public void SetBufferSize(int size)
        {
            Buffer = new byte[size];
        }
    }
}