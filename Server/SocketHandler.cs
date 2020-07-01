using System;
using System.Net;
using System.Net.Sockets;
using Entities;
using Entities.Commands;

namespace Server
{
    public class SocketHandler
    {
        public int Port { get; private set; }

        private Socket _socketListener;

        private IAsyncResult _acceptResult;

        public SocketHandler(int port)
        {
            Port = port;
        }

        public void Init()
        {
            _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListener.Bind(new IPEndPoint(IPAddress.Loopback, Port));
        }

        public void Listen()
        {
            if (_socketListener == null)
            {
                throw new InvalidOperationException("Socket is not initialized");
            }

            _socketListener.Listen(Constants.ServerMaximumBacklogSockets);
            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        public void OnAccept(IAsyncResult result)
        {
            try
            {
                var socketClient = _socketListener.EndAccept(_acceptResult);
                SocketData socketData = new SocketData(socketClient, sizeof(int));
                socketClient.BeginReceive(socketData.Buffer, 0, sizeof(int), SocketFlags.None, OnClientFirstReceive,
                    socketData);
            }
            catch (SocketException)
            {
                Console.WriteLine("Accepting client failed!");
            }

            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        private void OnClientFirstReceive(IAsyncResult ar)
        {
            SocketData socketData = (SocketData) ar.AsyncState;
            try
            {
                socketData.Socket.EndReceive(ar);

                int dataSize = BitConverter.ToInt32(socketData.Buffer, 0);
                socketData.SetBufferSize(dataSize); // расширяем буфер

                socketData.Socket.BeginReceive(socketData.Buffer, 0, dataSize, SocketFlags.None,
                    OnClientContinueReceive,
                    socketData);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client accidentaly disconnected");
                socketData.Socket.Close();
            }
        }

        private void OnClientContinueReceive(IAsyncResult ar)
        {
            SocketData socketData = (SocketData) ar.AsyncState;
            try
            {
                int requestDataSize = socketData.Socket.EndReceive(ar);
                socketData.ReceivedBytes += requestDataSize;
                if (socketData.ReceivedBytes == socketData.Buffer.Length)
                {
                    var responseCommand = ProcessRequest(socketData.Buffer);

                    var bytes = responseCommand.Serialize();

                    Utils.SendWithSizeHeader(socketData.Socket, bytes);

                    socketData.Socket.Shutdown(SocketShutdown.Both);
                    socketData.Socket.Close();
                }
                else
                {
                    socketData.Socket.BeginReceive(socketData.Buffer, socketData.ReceivedBytes,
                        socketData.Buffer.Length - socketData.ReceivedBytes, SocketFlags.None,
                        OnClientContinueReceive,
                        socketData);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Client accidentaly disconnected");
                socketData.Socket.Close();
            }
        }

        private Command ProcessRequest(byte[] data)
        {
            var socketCommand = Command.Deserialize(data);
            var command = ServerCommandHandler.Handle(socketCommand);
            return command;
        }
    }
}