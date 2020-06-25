using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketHandler handler = new SocketHandler(11771);
            handler.Init();
            handler.Listen();

            Console.ReadKey();
        }
    }
}
