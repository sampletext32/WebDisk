using System;
using System.IO;
using Entities;
using Entities.TreeNodes;

namespace Server
{
    public class ServerCommandHandler
    {
        private const string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug";
        private const string SharedFolderName = "shared";

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
            else if (command is UploadFilePieceCommand uploadFilePieceCommand)
            {
                Console.WriteLine("Performing UploadFilePiece");
                var filePieceData = uploadFilePieceCommand.GetData();
                FileStream fs;
                if (filePieceData.Offset == 0)
                {
                    fs = new FileStream(Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation,
                        filePieceData.Name), FileMode.Create);
                }
                else
                {
                    fs = new FileStream(Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation,
                        filePieceData.Name), FileMode.Append);
                }

                fs.Write(filePieceData.Data, 0, filePieceData.Size);
                if (filePieceData.Offset + filePieceData.Size == fs.Length)
                {
                    Console.WriteLine("Done File Upload");
                }

                fs.Close();

                return new EmptyCommand();
            }
            else if (command is IsFilesEqualCommand isFileDiffersCommand)
            {
                Console.WriteLine("Performing IsFilesEqualCommand");
                var fileComparationData = isFileDiffersCommand.GetData();
                string filePath = Path.Combine(SharedFolderLocation, fileComparationData.RelativeLocation,
                    fileComparationData.Name);
                if (File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    var localHash = TreeNode.CreateMD5(fs);
                    fs.Close();

                    if (localHash == fileComparationData.Hash)
                    {
                        return new ResponseIsFilesEqualCommand(true);
                    }
                    else
                    {
                        return new ResponseIsFilesEqualCommand(false);
                    }
                }
                else
                {
                    return new ResponseIsFilesEqualCommand(false);
                }

            }
            else if (command is CreateFolderCommand createFolderCommand)
            {
                Console.WriteLine("Performing CreateFolder");
                var createFolderData = createFolderCommand.GetData();
                if (!Directory.Exists(Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                    createFolderData.Name)))
                {
                    Directory.CreateDirectory(Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                        createFolderData.Name));
                }

                return new EmptyCommand();
            }
            else if (command is DeleteNonExistentCommand deleteNonExistentCommand)
            {
                Console.WriteLine("Performing CreateFolder");
                var remoteTree = deleteNonExistentCommand.GetData();
                remoteTree.DeleteNonExistent(SharedFolderLocation);
                return new EmptyCommand();
            }

            return new EmptyCommand();
        }
    }
}