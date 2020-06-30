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
                Console.WriteLine("Performing Hello");
                Console.WriteLine(helloCommand.GetData());
                return new EmptyCommand();
            }
            else if (command is CompareHashCommand compareHashCommand)
            {
                Console.WriteLine("Performing CompareHash");
                var treeEquals =
                    TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName)
                        .CalculateHash(SharedFolderLocation) == compareHashCommand.GetData();
                ResponseCompareHashCommand responseCompareCommand = new ResponseCompareHashCommand(treeEquals);
                return responseCompareCommand;
            }
            else if (command is GetTreeCommand getTreeCommand)
            {
                Console.WriteLine("Performing GetTree");
                ResponseGetTreeCommand responseGetTreeCommand =
                    new ResponseGetTreeCommand(TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName));
                return responseGetTreeCommand;
            }
            else if (command is GetFileSizeCommand getFileSizeCommand)
            {
                Console.WriteLine("Performing GetFileSize");
                var fileSizeData = getFileSizeCommand.GetData();
                FileInfo info = new FileInfo(Path.Combine(SharedFolderLocation, fileSizeData.RelativeLocation,
                    fileSizeData.Name));
                int size = (int) info.Length;
                ResponseGetFileSizeCommand responseGetFileSizeCommand = new ResponseGetFileSizeCommand(size);
                return responseGetFileSizeCommand;
            }
            else if (command is GetFilePieceCommand getFilePieceCommand)
            {
                Console.WriteLine("Performing GetFilePiece");
                var filePieceData = getFilePieceCommand.GetData();
                FileStream fs = new FileStream(Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation,
                    filePieceData.Name), FileMode.Open);
                fs.Seek(filePieceData.Offset, SeekOrigin.Begin);
                byte[] buffer = new byte[filePieceData.Size];
                fs.Read(buffer, 0, filePieceData.Size);
                fs.Close();
                ResponseGetFilePieceCommand responseGetFilePieceCommand = new ResponseGetFilePieceCommand(buffer);
                return responseGetFilePieceCommand;
            }

            return new EmptyCommand();
        }
    }
}