using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entities;

namespace Client
{
    public class SocketHandler : IRequestPerformer
    {
        private IPAddress _ipAddress;
        public int _port;
        private Socket _socket;

        private IAsyncResult _connectResult;

        private SocketHandler(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public static SocketHandler Request(IPAddress ipAddress, int port)
        {
            return new SocketHandler(ipAddress, port);
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
        }

        public T PerformCommand<T>(SocketCommand command) where T : SocketCommand
        {
            var bytes = command.Serialize();

            byte[] buffer = PerformRequest(bytes);

            var response = ProcessResponse<T>(buffer);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            return response;
        }

        private T ProcessResponse<T>(byte[] data) where T : SocketCommand
        {
            var socketCommand = SocketCommand.Deserialize(data);
            return (T) socketCommand;
        }

        public byte[] PerformRequest(byte[] data)
        {
            Init();
            Connect();

            while (!_connectResult.AsyncWaitHandle.WaitOne(10))
            {
                Thread.Sleep(1);
            }

            Utils.SendWithSizeHeader(_socket, data);

            byte[] buffer = Utils.ReceiveWithSizeHeader(_socket);
            return buffer;
        }
    }
}