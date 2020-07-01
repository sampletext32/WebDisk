using System;
using System.IO;
using Entities;
using Entities.Commands;
using Entities.TreeNodes;

namespace Server
{
    public class ServerCommandHandler
    {
        private const string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug";
        private const string SharedFolderName = "shared";

        public static Command Handle(Command command)
        {
            if (command is CommandSimpleMessage commandSimpleMessage)
            {
                Console.WriteLine("Performing Hello");
                Console.WriteLine(commandSimpleMessage.GetData());
                return new CommandNone();
            }
            else if (command is CommandCompareHash commandCompareHash)
            {
                Console.WriteLine("Performing CompareHash");
                var treeEquals =
                    TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName)
                        .CalculateHash(SharedFolderLocation) == commandCompareHash.GetData();
                CommandCompareHashResponse commandCompareHashResponse = new CommandCompareHashResponse(treeEquals);
                return commandCompareHashResponse;
            }
            else if (command is CommandGetTree commandGetTree)
            {
                Console.WriteLine("Performing GetTree");
                CommandGetTreeResponse commandGetTreeResponse =
                    new CommandGetTreeResponse(TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName));
                return commandGetTreeResponse;
            }
            else if (command is CommandGetFileSize commandGetFileSize)
            {
                Console.WriteLine("Performing GetFileSize");
                var fileSizeData = commandGetFileSize.GetData();
                FileInfo info = new FileInfo(Path.Combine(SharedFolderLocation, fileSizeData.RelativeLocation,
                    fileSizeData.Name));
                int size = (int) info.Length;
                CommandGetFileSizeResponse commandGetFileSizeResponse = new CommandGetFileSizeResponse(size);
                return commandGetFileSizeResponse;
            }
            else if (command is CommandGetFilePiece commandGetFilePiece)
            {
                Console.WriteLine("Performing GetFilePiece");
                var filePieceData = commandGetFilePiece.GetData();
                FileStream fs = new FileStream(Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation,
                    filePieceData.Name), FileMode.Open);
                fs.Seek(filePieceData.Offset, SeekOrigin.Begin);
                byte[] buffer = new byte[filePieceData.Size];
                fs.Read(buffer, 0, filePieceData.Size);
                fs.Close();
                CommandGetFilePieceResponse commandGetFilePieceResponse = new CommandGetFilePieceResponse(buffer);
                return commandGetFilePieceResponse;
            }
            else if (command is CommandUploadFilePiece commandUploadFilePiece)
            {
                Console.WriteLine("Performing UploadFilePiece");
                var filePieceData = commandUploadFilePiece.GetData();
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

                return new CommandNone();
            }
            else if (command is CommandIsFilesEqual commandIsFilesEqual)
            {
                Console.WriteLine("Performing SocketCommandIsFilesEqual");
                var fileComparisonData = commandIsFilesEqual.GetData();
                string filePath = Path.Combine(SharedFolderLocation, fileComparisonData.RelativeLocation,
                    fileComparisonData.Name);
                if (File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    var localHash = TreeNode.CreateMD5(fs);
                    fs.Close();

                    if (localHash == fileComparisonData.Hash)
                    {
                        return new CommandIsFilesEqualResponse(true);
                    }
                    else
                    {
                        return new CommandIsFilesEqualResponse(false);
                    }
                }
                else
                {
                    return new CommandIsFilesEqualResponse(false);
                }

            }
            else if (command is CommandCreateFolder commandCreateFolder)
            {
                Console.WriteLine("Performing CreateFolder");
                var createFolderData = commandCreateFolder.GetData();
                if (!Directory.Exists(Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                    createFolderData.Name)))
                {
                    Directory.CreateDirectory(Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                        createFolderData.Name));
                }

                return new CommandNone();
            }
            else if (command is CommandDeleteNonExistent commandDeleteNonExistent)
            {
                Console.WriteLine("Performing CreateFolder");
                var remoteTree = commandDeleteNonExistent.GetData();
                remoteTree.DeleteNonExistent(SharedFolderLocation);
                return new CommandNone();
            }

            return new CommandNone();
        }
    }
}