using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Entities;
using Entities.SocketCommands;

namespace Client
{
    public class SocketHandler : IRequestPerformer
    {
        public IPAddress IpAddress { get; private set; }
        public int Port { get; set; }

        private Socket _socket;

        private IAsyncResult _connectResult;

        private SocketHandler(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public static SocketHandler Request(IPAddress ipAddress, int port)
        {
            return new SocketHandler(ipAddress, port);
        }

        public void Init()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Connect()
        {
            _connectResult = _socket.BeginConnect(IpAddress, Port, ar => { }, null);

            while (!_connectResult.AsyncWaitHandle.WaitOne(Constants.ConnectionTimeoutMilliseconds))
            {
                Thread.Sleep(1);
            }

            try
            {
                _socket.EndConnect(_connectResult);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T PerformCommand<T>(SocketCommand command) where T : SocketCommand
        {
            var bytes = command.Serialize();

            byte[] buffer = PerformRequest(bytes);

            var response = ProcessResponse<T>(buffer);

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
            if (Connect())
            {
                Utils.SendWithSizeHeader(_socket, data);

                byte[] buffer = Utils.ReceiveWithSizeHeader(_socket);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();

                return buffer;
            }
            else
            {
                throw new InvalidOperationException("Connect Failed!");
            }
        }
    }
}