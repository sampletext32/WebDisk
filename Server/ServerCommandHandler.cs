using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.TreeNodes;

namespace Server
{
    public class ServerCommandHandler
    {
        private static string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug";
        private static string SharedFolderName = "shared";

        public static SocketCommand Handle(SocketCommand command)
        {
            if (command is HelloCommand helloCommand)
            {
                Console.WriteLine(helloCommand.GetData());
                return new EmptyCommand();
            }
            else if (command is CompareHashCommand compareHashCommand)
            {
                var treeEquals = TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName).CalculateHash(SharedFolderLocation) == compareHashCommand.GetData();
                ResponseCompareHashCommand responseCompareCommand = new ResponseCompareHashCommand(treeEquals);
                return responseCompareCommand;
            }
            else if (command is GetTreeCommand getTreeCommand)
            {
                ResponseGetTreeCommand responseGetTreeCommand = new ResponseGetTreeCommand(TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName));
                return responseGetTreeCommand;
            }
            else if (command is GetHashXmlTreeCommand getHashXmlTreeCommand)
            {
                ResponseGetHashXmlTreeCommand responseGetHashXmlTreeCommand = new ResponseGetHashXmlTreeCommand(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                    TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName).WrapHashedXML(SharedFolderLocation));
                return responseGetHashXmlTreeCommand;
            }

            return new EmptyCommand();
        }
    }
}