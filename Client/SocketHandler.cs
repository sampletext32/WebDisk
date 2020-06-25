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
        }

        public T PerformCommand<T>(SocketCommand command)
        {
            var bytes = command.Serialize();

            Init();
            Connect();

            while (!_connectResult.AsyncWaitHandle.WaitOne(10))
            {
                Thread.Sleep(1);
            }

            Utils.SendWithSizeHeader(_socket, bytes);

            byte[] buffer = Utils.ReceiveWithSizeHeader(_socket);

            var response = ProcessResponse<T>(buffer);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            return response;
        }

        private T ProcessResponse<T>(byte[] data)
        {
            var socketCommand = SocketCommand.Deserialize(data);
            var responseCommand = ClientCommandHandler.Upcast(socketCommand);
            return (T)responseCommand;
        }
    }
}