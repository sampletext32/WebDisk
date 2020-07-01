using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Entities
{
    public class Utils
    {
        public static void SendWithSizeHeader(Socket socket, byte[] data)
        {
            if (Constants.Debug) Console.WriteLine($"SendWithSizeHeader DataSize: {{ {data.Length} }};");

            socket.Send(BitConverter.GetBytes(data.Length), 0, sizeof(int), SocketFlags.None);

            int sent = 0;
            while (sent < data.Length)
            {
                sent += socket.Send(data, sent, data.Length - sent, SocketFlags.None);
            }
        }

        public static byte[] ReceiveWithSizeHeader(Socket socket)
        {
            if (Constants.Debug) Console.WriteLine($"ReceiveWithSizeHeader Called;");

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
            if (Constants.Debug) Console.WriteLine($"PathIsDirectory path: {{ {path} }};");

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

        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string CreateMD5(Stream input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(input);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}