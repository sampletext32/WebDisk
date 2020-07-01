using System;
using Entities;

namespace Server
{
    class Program
    {
        // запуск сервера
        static void Main(string[] args)
        {
            // создаём обработчик
            SocketHandler handler = new SocketHandler(Constants.ConnectionPort);
            
            // инициализируем и запускаем
            handler.Init();
            handler.Listen();

            Console.WriteLine("Started Server");

            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
    }
}