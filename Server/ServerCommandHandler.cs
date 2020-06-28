using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server
{
    public class ServerCommandHandler
    {
        private static SharedFolder _sharedFolder =
            new SharedFolder("C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug\\shared");

        public static SocketCommand Handle(SocketCommand command)
        {
            if (command is HelloCommand helloCommand)
            {
                Console.WriteLine(helloCommand.GetData());
                return new EmptyCommand();
            }
            else if (command is CompareHashCommand compareHashCommand)
            {
                var path = _sharedFolder.GetPath();
                var sharedFolderRoot = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
                var treeEquals = _sharedFolder.AsTreeNode().CalculateHash(sharedFolderRoot) == compareHashCommand.GetData();
                ResponseCompareHashCommand responseCompareCommand = new ResponseCompareHashCommand(treeEquals);
                return responseCompareCommand;
            }
            else if (command is GetTreeCommand getTreeCommand)
            {
                ResponseGetTreeCommand responseGetTreeCommand = new ResponseGetTreeCommand(_sharedFolder.AsTreeNode());
                return responseGetTreeCommand;
            }
            else if (command is GetHashXmlTreeCommand getHashXmlTreeCommand)
            {
                var path = _sharedFolder.GetPath();
                var sharedFolderRoot = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
                ResponseGetHashXmlTreeCommand responseGetHashXmlTreeCommand = new ResponseGetHashXmlTreeCommand(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                    _sharedFolder.AsTreeNode().WrapHashedXML(sharedFolderRoot));
                return responseGetHashXmlTreeCommand;
            }

            return new EmptyCommand();
        }
    }
}