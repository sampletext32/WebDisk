using System;
using System.Collections.Generic;
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
        public static string Path;
        private static SocketHandler Handler;

        static void Main(string[] args)
        {
            Path = "C:\\Projects\\CSharp\\WebDisk\\Client\\bin\\Debug\\shared";
            Handler = new SocketHandler(IPAddress.Loopback, 11771);

            Thread syncThread = new Thread(ThreadFunc);
            syncThread.Start();

            // HelloCommand command = new HelloCommand("Hello, i am migga nigga");
            // Handler.PerformCommand(command);

            Console.ReadKey();

            syncThread.Abort();
        }

        public static void ThreadFunc()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                TreeAnalyzer analyzer = new TreeAnalyzer(Path);
                analyzer.Retrieve();
                var treeNode = analyzer.GetTree();

                var hash = treeNode.GetHash();

                CompareHashCommand command = new CompareHashCommand(hash);
                var commandResult = Handler.PerformCommand<ResponseCompareHashCommand>(command);
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