using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Entities;
using Entities.SocketCommands;

namespace Server
{
    public class SocketHandler
    {
        public int Port { get; private set; }

        private Socket _socketListener;

        private List<SocketData> _connectedClients;

        private IAsyncResult _acceptResult;

        public SocketHandler(int port)
        {
            Port = port;
            _connectedClients = new List<SocketData>();
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
                _connectedClients.Add(socketData);
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
                byte[] data = socketData.Buffer;
                int dataSize = BitConverter.ToInt32(data, 0);
                socketData.SetBufferSize(dataSize);

                socketData.Socket.BeginReceive(socketData.Buffer, 0, dataSize, SocketFlags.None,
                    OnClientContinueReceive,
                    socketData);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client accidentaly disconnected");
                socketData.Socket.Close();
                _connectedClients.Remove(socketData);
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
                    _connectedClients.Remove(socketData);
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
                _connectedClients.Remove(socketData);
            }
        }

        private SocketCommand ProcessRequest(byte[] data)
        {
            var socketCommand = SocketCommand.Deserialize(data);
            var command = ServerCommandHandler.Handle(socketCommand);
            return command;
        }
    }
}