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
        private const string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Client\\bin\\Debug";
        private const string SharedFolderName = "shared";

        static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(SharedFolderLocation, SharedFolderName)))
            {
                Console.WriteLine($"{SharedFolderName} folder not found");

                var treeCommand = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort)
                    .PerformCommand<CommandGetTreeResponse>(new CommandGetTree());

                var remoteTreeRoot = treeCommand.GetData();
                remoteTreeRoot.Download(SharedFolderLocation,
                    SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);
            }
            else
            {
                Console.WriteLine($"Found {SharedFolderName} folder");
            }

            Console.WriteLine("Starting sync thread");

            Thread syncThread = new Thread(ThreadFunc);
            syncThread.Start();

            // CommandSimpleMessage command = new CommandSimpleMessage("Hello, i am migga nigga");
            // Handler.PerformCommand(command);

            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();

            syncThread.Abort();
        }

        public static void ThreadFunc()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                var localTree = TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName);

                var hash = localTree.CalculateHash(SharedFolderLocation);

                CommandCompareHash socketCommandCompareHash = new CommandCompareHash(hash);
                var socketHandler = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort);
                var responseCompareHashCommand =
                    socketHandler.PerformCommand<CommandCompareHashResponse>(socketCommandCompareHash);
                if (responseCompareHashCommand.GetData() == true)
                {
                    Console.WriteLine("Folders equals");
                }
                else
                {
                    Console.WriteLine("Found mismatch, performing sync");

                    Console.WriteLine("Deleting folders");
                    CommandDeleteNonExistent socketCommandDeleteNonExistent = new CommandDeleteNonExistent(localTree);
                    var deleteNonExistentCommandResponse =
                        socketHandler.PerformCommand<CommandNone>(socketCommandDeleteNonExistent);

                    Console.WriteLine("Uploading tree");
                    localTree.Upload(SharedFolderLocation, SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);
                }

                Thread.Sleep(10 * 1000);
            }
        }
    }
}