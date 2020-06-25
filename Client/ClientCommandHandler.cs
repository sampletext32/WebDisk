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
        public static object Upcast(SocketCommand command)
        {
            if (command is ResponseCompareHashCommand responseCompareHashCommand)
            {
                return responseCompareHashCommand;
            }

            return null;
        }
    }
}