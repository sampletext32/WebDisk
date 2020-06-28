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
        public static string AbsoluteFolderPath = "C:\\Projects\\CSharp\\WebDisk\\Client\\bin\\Debug\\shared";

        static void Main(string[] args)
        {
            if (!Directory.Exists("shared"))
            {
                Console.WriteLine("shared folder not found");
                Directory.CreateDirectory("shared");
                // TODO: Full download
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
                TreeAnalyzer analyzer = new TreeAnalyzer();
                analyzer.Retrieve(AbsoluteFolderPath);
                var localTree = analyzer.GetTree();

                var hash = localTree.GetHash(AbsoluteFolderPath);

                CompareHashCommand command = new CompareHashCommand(hash);
                var socketHandler = SocketHandler.Request(IPAddress.Loopback, 11771);
                var commandResult = socketHandler.PerformCommand<ResponseCompareHashCommand>(command);
                if (commandResult.GetData() == true)
                {
                    Console.WriteLine("Folders equals");
                }
                else
                {
                    Console.WriteLine("Found mismatch, perform sync");
                    // TODO: Sync
                }

                Thread.Sleep(10 * 1000);
            }
        }
    }
}