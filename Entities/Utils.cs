using System;
using System.IO;
using System.Net.Sockets;

namespace Entities
{
    public class Utils
    {
        public static void SendWithSizeHeader(Socket socket, byte[] data)
        {
            socket.Send(BitConverter.GetBytes(data.Length), 0, sizeof(int), SocketFlags.None);

            int sent = 0;
            while (sent < data.Length)
            {
                sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
            }
        }

        public static byte[] ReceiveWithSizeHeader(Socket socket)
        {
            byte[] buffer = new byte[4];
            socket.Receive(buffer, 0, sizeof(int), SocketFlags.None);
            int dataSize = BitConverter.ToInt32(buffer, 0);

            buffer = new byte[dataSize];

            int received = 0;
            while (received < dataSize)
            {
                received += socket.Receive(buffer, received, dataSize - received, SocketFlags.None);
            }

            return buffer;
        }
        
        public static bool PathIsDirectory(string path)
        {
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                return fa.HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }
    }
}