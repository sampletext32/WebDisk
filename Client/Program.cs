using System;
using System.IO;
using System.Net;
using System.Threading;
using Entities;
using Entities.Commands;
using Entities.TreeNodes;

namespace Client
{
    class Program
    {
        // положение общедоступной папки клиента
        private const string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Client\\bin\\Debug";

        // название общедоступной папки клиента (не рекомендуется менять!)
        private const string SharedFolderName = "shared";

        static void Main(string[] args)
        {
            // если папки не существует
            if (!Directory.Exists(Path.Combine(SharedFolderLocation, SharedFolderName)))
            {
                if (Constants.Debug) Console.WriteLine($"Shared Folder Not Found: {{ {SharedFolderName} in {SharedFolderLocation} }};");

                if (Constants.Debug) Console.WriteLine($"Started Tree Downloading;");

                // скачиваем само серверное дерево
                var treeCommand = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort)
                    .PerformCommand<CommandGetTreeResponse>(new CommandGetTree());

                var remoteTreeRoot = treeCommand.GetData();

                // скачиваем контент серверного дерева
                remoteTreeRoot.Download(SharedFolderLocation,
                    SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);

                if (Constants.Debug) Console.WriteLine($"Finished Tree Downloading;");
            }
            else
            {
                if (Constants.Debug) Console.WriteLine($"Shared Folder Found;");
            }

            if (Constants.Debug) Console.WriteLine($"Starting Sync Thread;");

            // запускаем поток синхронизации
            Thread syncThread = new Thread(SyncThreadJob);
            syncThread.Start();

            Console.WriteLine("Started Client");

            // CommandSimpleMessage command = new CommandSimpleMessage("Hello, i am migga nigga");
            // Handler.PerformCommand(command);

            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();

            if (Constants.Debug) Console.WriteLine($"Aborting Sync Thread;");

            // не забываем завершить поток
            syncThread.Abort();

            if (Constants.Debug) Console.WriteLine($"Finished;");
        }

        // работа потока синхронизации
        public static void SyncThreadJob()
        {
            // пока не запросили отмену потока
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                if (Constants.Debug) Console.WriteLine($"Sync Thread Pulse Start;");

                // строим локальное дерево
                var localTree = TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName);

                // строим хеш локального дерева
                var hash = localTree.CalculateHash(SharedFolderLocation);
                
                if (Constants.Debug) Console.WriteLine($"Perform CompareHash Command;");

                // создаём и выполняем команду
                CommandCompareHash socketCommandCompareHash = new CommandCompareHash(hash);
                var socketHandler = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort);
                var responseCompareHashCommand =
                    socketHandler.PerformCommand<CommandCompareHashResponse>(socketCommandCompareHash);

                if (responseCompareHashCommand.GetData() == true)
                {
                    if (Constants.Debug) Console.WriteLine($"Sync Pulse Everything Up-to-date;");
                    // если удалённое дерево такое же как локальное - ничего не делаем
                }
                else
                {
                    if (Constants.Debug) Console.WriteLine($"Sync Pulse Found Mismatch;");

                    if (Constants.Debug) Console.WriteLine($"Perform DeleteNonExistent Command;");

                    // если удалённое дерево отличается

                    // запускаем удаление локально несуществующих файлов на сервере (отправляем локальное дерево)
                    CommandDeleteNonExistent socketCommandDeleteNonExistent = new CommandDeleteNonExistent(localTree);
                    var deleteNonExistentCommandResponse =
                        socketHandler.PerformCommand<CommandNone>(socketCommandDeleteNonExistent);

                    if (Constants.Debug) Console.WriteLine($"Perform UploadTree Command;");

                    // выгружаем само дерево (к этому моменту структура папок уже синхронизирована, а новые папки создадутся)
                    localTree.Upload(SharedFolderLocation, SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);
                }

                if (Constants.Debug) Console.WriteLine($"Sync Thread Pulse Finished, Waiting;");

                // ожидаем задержку между синхронизациями
                Thread.Sleep(Constants.ClientSynchronizationDelayMilliseconds);
            }
        }
    }
}