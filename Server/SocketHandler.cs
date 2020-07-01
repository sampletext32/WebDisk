using System;
using System.Net;
using System.Net.Sockets;
using Entities;
using Entities.Commands;

namespace Server
{
    // обработчик сокета сервера
    public class SocketHandler
    {
        // порт прослушки
        public int Port { get; private set; }

        // слушающий сокет
        private Socket _socketListener;

        // асинхронный результат подключения клиента
        private IAsyncResult _acceptResult;

        public SocketHandler(int port)
        {
            Port = port;
        }

        public void Init()
        {
            // создаём сокет и связываем с локальной точкой
            _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListener.Bind(new IPEndPoint(IPAddress.Loopback, Port));
        }

        public void Listen()
        {
            if (_socketListener == null)
            {
                // если вдруг случайно вызвали прослушку на неинициализированном сокете
                throw new InvalidOperationException("Socket is not initialized");
            }

            // запускаем прослушку
            _socketListener.Listen(Constants.ServerMaximumBacklogSockets);

            // асинхронно подключаем клиента, обрабатывая его в OnAccept
            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        // обработка подключения клиента
        public void OnAccept(IAsyncResult result)
        {
            // пробуем подключить - если ошибка - клиент потерян
            try
            {
                // завершаем подключения клиента
                var socketClient = _socketListener.EndAccept(_acceptResult);

                // создаём объект состояния
                SocketData socketData = new SocketData(socketClient, sizeof(int));

                // запускаем получения передаём объект состояни клиента и обрабатываем его в OnClientFirstReceive (первое получение)
                socketClient.BeginReceive(socketData.Buffer, 0, sizeof(int), SocketFlags.None, OnClientFirstReceive,
                    socketData);
            }
            catch (SocketException)
            {
                Console.WriteLine("Accepting client failed!");
            }

            // после любого подключения обязательно вызываем обработку следующих
            _acceptResult = _socketListener.BeginAccept(OnAccept, null);
        }

        // первое получение данных от клиента
        private void OnClientFirstReceive(IAsyncResult ar)
        {
            // достаём объект состояния клиента из операции
            SocketData socketData = (SocketData) ar.AsyncState;

            // пробуем получить данные - если ошибка - клиент как-то умудрился отключился до первой отправки
            try
            {
                // завершаем получение
                socketData.Socket.EndReceive(ar);

                // узнаём, сколько данных отправил клиент
                int dataSize = BitConverter.ToInt32(socketData.Buffer, 0);
                socketData.SetBufferSize(dataSize); // расширяем буфер

                // запускаем получения всего буфера данных (обрабатываем в OnClientContinueReceive), снова передаём объект состояния
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

        // любое получение данных данных от клиента, кроме первого
        private void OnClientContinueReceive(IAsyncResult ar)
        {
            // достаём объект состояния клиента из операции
            SocketData socketData = (SocketData) ar.AsyncState;

            // пробуем получить данные - если ошибка - клиент отключился
            try
            {
                // завершаем получение
                int requestDataSize = socketData.Socket.EndReceive(ar);

                // увеличиваем количество полученных данных
                socketData.ReceivedBytes += requestDataSize;

                // если скачали всё
                if (socketData.ReceivedBytes == socketData.Buffer.Length)
                {
                    // запускаем обработку пакета
                    var responseCommand = ProcessRequest(socketData.Buffer);

                    // запаковываем ответ
                    var bytes = responseCommand.Serialize();

                    // отправляем ответ
                    Utils.SendWithSizeHeader(socketData.Socket, bytes);

                    // после отпраки обязательно отключаем сокет и закрываем
                    socketData.Socket.Shutdown(SocketShutdown.Both);
                    socketData.Socket.Close();
                }
                else
                {
                    // Если скачали ещё не все
                    // запускаем получения оставшегося буфера данных (обрабатываем в OnClientContinueReceive), снова передаём объект состояния
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

        // метод обработки пакетов клиента
        private Command ProcessRequest(byte[] data)
        {
            // распаковываем команду
            var socketCommand = Command.Deserialize(data);

            // обрабатываем команду и формируем ответ
            var command = ServerCommandHandler.Handle(socketCommand);

            // возвращаем ответ
            return command;
        }
    }
}