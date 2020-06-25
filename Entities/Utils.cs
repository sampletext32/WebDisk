using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Utils
    {
        public static void SendWithSizeHeader(Socket socket, byte[] data)
        {
            socket.Send(BitConverter.GetBytes(data.Length), 0, 4, SocketFlags.None);
            socket.Send(data, 0, data.Length, SocketFlags.None);
        }

        public static byte[] ReceiveWithSizeHeader(Socket socket)
        {
            byte[] buffer = new byte[4];
            socket.Receive(buffer, 0, 4, SocketFlags.None);
            int dataSize = BitConverter.ToInt32(buffer, 0);
            buffer = new byte[dataSize];
            socket.Receive(buffer, 0, dataSize, SocketFlags.None);
            return buffer;
        }
    }
}