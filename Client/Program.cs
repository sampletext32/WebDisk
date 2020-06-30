using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Entities.TreeNodes;

namespace Client
{
    class Program
    {
        public static readonly string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Client\\bin\\Debug";
        public static readonly string SharedFolderName = "shared";

        static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(SharedFolderLocation, SharedFolderName)))
            {
                Console.WriteLine($"{SharedFolderName} folder not found");

                var treeCommand = SocketHandler.Request(IPAddress.Loopback, 11771)
                    .PerformCommand<ResponseGetTreeCommand>(new GetTreeCommand());

                var remoteTreeRoot = treeCommand.GetData();
                remoteTreeRoot.Download(SharedFolderLocation,
                    SocketHandler.Request(IPAddress.Loopback, 11771), false);
            }
            else
            {
                Console.WriteLine($"Found {SharedFolderName} folder");
            }

            Console.WriteLine("Starting sync thread");

            Thread syncThread = new Thread(ThreadFunc);
            syncThread.Start();

            // HelloCommand command = new HelloCommand("Hello, i am migga nigga");
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

                CompareHashCommand compareHashCommand = new CompareHashCommand(hash);
                var socketHandler = SocketHandler.Request(IPAddress.Loopback, 11771);
                var responseCompareHashCommand =
                    socketHandler.PerformCommand<ResponseCompareHashCommand>(compareHashCommand);
                if (responseCompareHashCommand.GetData() == true)
                {
                    Console.WriteLine("Folders equals");
                }
                else
                {
                    Console.WriteLine("Found mismatch, performing sync");

                    localTree.Upload(SharedFolderLocation, SocketHandler.Request(IPAddress.Loopback, 11771), false);
                }

                Thread.Sleep(10 * 1000);
            }
        }
    }
}