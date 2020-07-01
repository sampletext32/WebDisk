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
                if (Constants.Debug) Console.WriteLine($"Shared Folder Not Found: {{ {SharedFolderName} in {SharedFolderLocation} }};");

                if (Constants.Debug) Console.WriteLine($"Started Tree Downloading;");

                var treeCommand = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort)
                    .PerformCommand<CommandGetTreeResponse>(new CommandGetTree());

                var remoteTreeRoot = treeCommand.GetData();
                remoteTreeRoot.Download(SharedFolderLocation,
                    SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);

                if (Constants.Debug) Console.WriteLine($"Finished Tree Downloading;");
            }
            else
            {
                if (Constants.Debug) Console.WriteLine($"Shared Folder Found;");
            }

            if (Constants.Debug) Console.WriteLine($"Starting Sync Thread;");

            Thread syncThread = new Thread(SyncThreadJob);
            syncThread.Start();

            Console.WriteLine("Started Client");

            // CommandSimpleMessage command = new CommandSimpleMessage("Hello, i am migga nigga");
            // Handler.PerformCommand(command);

            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();

            if (Constants.Debug) Console.WriteLine($"Aborting Sync Thread;");

            syncThread.Abort();

            if (Constants.Debug) Console.WriteLine($"Finished;");
        }

        public static void SyncThreadJob()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                if (Constants.Debug) Console.WriteLine($"Sync Thread Pulse Start;");

                var localTree = TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName);

                var hash = localTree.CalculateHash(SharedFolderLocation);
                
                if (Constants.Debug) Console.WriteLine($"Perform CompareHash Command;");

                CommandCompareHash socketCommandCompareHash = new CommandCompareHash(hash);
                var socketHandler = SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort);
                var responseCompareHashCommand =
                    socketHandler.PerformCommand<CommandCompareHashResponse>(socketCommandCompareHash);
                if (responseCompareHashCommand.GetData() == true)
                {
                    if (Constants.Debug) Console.WriteLine($"Sync Pulse Everything Up-to-date;");
                }
                else
                {
                    if (Constants.Debug) Console.WriteLine($"Sync Pulse Found Mismatch;");

                    if (Constants.Debug) Console.WriteLine($"Perform DeleteNonExistent Command;");

                    CommandDeleteNonExistent socketCommandDeleteNonExistent = new CommandDeleteNonExistent(localTree);
                    var deleteNonExistentCommandResponse =
                        socketHandler.PerformCommand<CommandNone>(socketCommandDeleteNonExistent);

                    if (Constants.Debug) Console.WriteLine($"Perform UploadTree Command;");

                    localTree.Upload(SharedFolderLocation, SocketHandler.Request(IPAddress.Loopback, Constants.ConnectionPort), false);
                }

                if (Constants.Debug) Console.WriteLine($"Sync Thread Pulse Finished, Waiting;");

                Thread.Sleep(10 * 1000);
            }
        }
    }
}