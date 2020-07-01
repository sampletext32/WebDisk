using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Entities;
using Entities.Commands;

namespace Client
{
    // обработчик подключения клиента (реализует интерфейс выполнителя запросов)
    public class SocketHandler : IRequestPerformer
    {
        // IP адрес сервера 
        public IPAddress IpAddress { get; private set; }

        // порт сервера
        public int Port { get; private set; }

        // сокет
        private Socket _socket;

        private SocketHandler(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        // статический обработчик (для удобства пользования (паттерн декоратор), равен вызову конструктора)
        public static SocketHandler Request(IPAddress ipAddress, int port)
        {
            return new SocketHandler(ipAddress, port);
        }

        // инициализация сокета
        public void Init()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        // подключение к серверу
        public bool Connect()
        {
            // начинаем подключение к сервера
            IAsyncResult connectResult = _socket.BeginConnect(IpAddress, Port, ar => { }, null);

            // ожидаем подключение в течение заданного интервала 
            connectResult.AsyncWaitHandle.WaitOne(Constants.ConnectionTimeoutMilliseconds);

            // после ожидания пробуем завершить подключение
            try
            {
                _socket.EndConnect(connectResult);
                return true;
            }
            catch
            {
                // ошибка - сокет не подключился
                return false;
            }
        }

        // метод выполнения команды (Возвращает заданную команду T)
        public T PerformCommand<T>(Command command) where T : Command
        {
            // запаковываем ответ
            var bytes = command.Serialize();

            // выполняем запрос
            byte[] buffer = PerformRequest(bytes);

            // распаковываем ответ
            var response = ProcessResponse<T>(buffer);

            // возвращаем ответ
            return response;
        }

        // метод для распаковки ответа
        private T ProcessResponse<T>(byte[] data) where T : Command
        {
            // десериализуем данные
            var socketCommand = Command.Deserialize(data); 
            return (T) socketCommand;
        }

        // обработка запроса
        public byte[] PerformRequest(byte[] data)
        {
            // инициализируем сокет
            Init();

            // если удалось подключиться
            if (Connect())
            {
                // отправляем данные
                Utils.SendWithSizeHeader(_socket, data);

                // получаем данные
                byte[] buffer = Utils.ReceiveWithSizeHeader(_socket);

                // отключаем закрываем сокет
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