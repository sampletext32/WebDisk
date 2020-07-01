﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using Entities;
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
                    .PerformCommand<ResponseGetTreeCommand>(new GetTreeCommand());

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

                    Console.WriteLine("Deleting folders");
                    DeleteNonExistentCommand deleteNonExistentCommand = new DeleteNonExistentCommand(localTree);
                    var deleteNonExistentCommandResponse =
                        socketHandler.PerformCommand<EmptyCommand>(deleteNonExistentCommand);

                    Console.WriteLine("Uploading tree");
                    localTree.Upload(SharedFolderLocation, SocketHandler.Request(IPAddress.Loopback, 11771), false);
                }

                Thread.Sleep(10 * 1000);
            }
        }
    }
}