using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SocketHandler
    {
        private IPAddress _ipAddress;
        public int _port;
        private Socket _socket;
        private byte[] _buffer;

        private IAsyncResult _connectResult;

        public SocketHandler(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void Init()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            _connectResult = _socket.BeginConnect(_ipAddress, _port, OnConnect, null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            _socket.EndConnect(_connectResult);

            string testData = "Hello, i am client";
            var bytes = Encoding.UTF8.GetBytes(testData);
            //TODO: Send data size
            _socket.Send(BitConverter.GetBytes(bytes.Length), 0, 4, SocketFlags.None);
            //TODO: Send request
            _socket.Send(bytes, 0, bytes.Length, SocketFlags.None);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}