using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketHandler handler = new SocketHandler(IPAddress.Loopback, 11771);
            handler.Init();
            handler.Connect();
            Console.ReadKey();
        }
    }
}
