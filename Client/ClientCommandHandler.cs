using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Client
{
    public class ClientCommandHandler
    {
        public static object Handle(SocketCommand command)
        {
            if (command is ResponseCompareHashCommand responseCompareHashCommand)
            {
                var foldersEquals = responseCompareHashCommand.GetData();
                Console.WriteLine("FoldersEquals: {0}", foldersEquals);
                return foldersEquals;
            }

            return null;
        }
    }
}