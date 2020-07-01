using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketHandler handler = new SocketHandler(Constants.ConnectionPort);
            handler.Init();
            handler.Listen();

            Console.ReadKey();
        }
    }
}