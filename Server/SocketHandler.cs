using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server
{
    public class SocketHandler
    {
        private int _port;
        private Socket _socketListener;

        private List<SocketData> _datas;

        private IAsyncResult _acceptResult;

        public SocketHandler(int port)
        {
            _port = port;
            _datas = new List<SocketData>();
        }

        public void Init()
        {
            _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListener.Bind(new IPEndPoint(IPAddress.Loopback, _port));
        }

        public void Listen()
        {
            if (_socketListener == null)
            {
                throw new InvalidOperationException("Socket is not initialized");
            }

            _socketListener.Listen(10);
            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        public void OnAccept(IAsyncResult result)
        {
            var socketClient = _socketListener.EndAccept(_acceptResult);
            SocketData socketData = new SocketData(socketClient, 4);
            socketClient.BeginReceive(socketData.Buffer, 0, 4, SocketFlags.None, OnClientFirstReceive, socketData);
            _datas.Add(socketData);

            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        private void OnClientFirstReceive(IAsyncResult ar)
        {
            SocketData socketData = (SocketData) ar.AsyncState;
            socketData.Socket.EndReceive(ar);
            byte[] data = socketData.Buffer;
            int dataSize = BitConverter.ToInt32(data, 0);
            socketData.SetBufferSize(dataSize);

            socketData.Socket.BeginReceive(socketData.Buffer, 0, dataSize, SocketFlags.None, OnClientContinueReceive,
                socketData);
        }

        private void OnClientContinueReceive(IAsyncResult ar)
        {
            SocketData socketData = (SocketData) ar.AsyncState;
            int requestDataSize = socketData.Socket.EndReceive(ar);
            socketData.ReceivedBytes += requestDataSize;
            if (socketData.ReceivedBytes == socketData.Buffer.Length)
            {
                var responseCommand = ProcessData(socketData.Buffer);

                var bytes = responseCommand.Serialize();

                // Send size
                socketData.Socket.Send(BitConverter.GetBytes(bytes.Length), 0, 4, SocketFlags.None);
                // Send data
                socketData.Socket.Send(bytes, 0, bytes.Length, SocketFlags.None);
                
                socketData.Socket.Shutdown(SocketShutdown.Both);
                socketData.Socket.Close();
                _datas.Remove(socketData);
            }
            else
            {
                socketData.Socket.BeginReceive(socketData.Buffer, socketData.ReceivedBytes, 8192, SocketFlags.None,
                    OnClientContinueReceive,
                    socketData);
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