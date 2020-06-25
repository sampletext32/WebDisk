using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server
{
    public class ServerCommandHandler
    {
        private static SharedFolder _sharedFolder = new SharedFolder("C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug\\shared");

        public static SocketCommand Handle(SocketCommand command)
        {
            if (command is HelloCommand helloCommand)
            {
                Console.WriteLine(helloCommand.GetData());
                return new EmptyCommand();
            }
            else if (command is CompareHashCommand compareHashCommand)
            {
                var treeEquals = _sharedFolder.AsTreeNode().GetHash() == compareHashCommand.GetData();
                ResponseCompareHashCommand responseCompareCommand = new ResponseCompareHashCommand(treeEquals);
                return responseCompareCommand;
            }
            return new EmptyCommand();
        }
    }
}