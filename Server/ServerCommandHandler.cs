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
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandSimpleMessage }");

                Console.WriteLine(commandSimpleMessage.GetData());
                return new CommandNone();
            }
            else if (command is CommandCompareHash commandCompareHash)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandCompareHash }");

                var treeEquals =
                    TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName)
                        .CalculateHash(SharedFolderLocation) == commandCompareHash.GetData();
                CommandCompareHashResponse commandCompareHashResponse = new CommandCompareHashResponse(treeEquals);
                return commandCompareHashResponse;
            }
            else if (command is CommandGetTree commandGetTree)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetTree }");

                CommandGetTreeResponse commandGetTreeResponse =
                    new CommandGetTreeResponse(TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName));
                return commandGetTreeResponse;
            }
            else if (command is CommandGetFileSize commandGetFileSize)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetFileSize }");

                var fileSizeData = commandGetFileSize.GetData();
                FileInfo info = new FileInfo(Path.Combine(SharedFolderLocation, fileSizeData.RelativeLocation,
                    fileSizeData.Name));
                int size = (int) info.Length;
                CommandGetFileSizeResponse commandGetFileSizeResponse = new CommandGetFileSizeResponse(size);
                return commandGetFileSizeResponse;
            }
            else if (command is CommandGetFilePiece commandGetFilePiece)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetFilePiece }");

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
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandUploadFilePiece }");

                var filePieceData = commandUploadFilePiece.GetData();
                var path = Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation, filePieceData.Name);
                FileStream fs;
                if (filePieceData.Offset == 0)
                {
                    Console.WriteLine($"Server created file: {{ {filePieceData.Name} }}");
                    fs = new FileStream(path, FileMode.Create);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Append);
                }

                fs.Write(filePieceData.Data, 0, filePieceData.Size);
                fs.Flush(true);

                fs.Close();

                return new CommandNone();
            }
            else if (command is CommandIsFilesEqual commandIsFilesEqual)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandIsFilesEqual }");

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
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandCreateFolder }");

                var createFolderData = commandCreateFolder.GetData();
                if (!Directory.Exists(Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                    createFolderData.Name)))
                {
                    var path = Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation,
                        createFolderData.Name);
                    Directory.CreateDirectory(path);

                    Console.WriteLine(
                        $"Server created folder: {{ {Path.Combine(createFolderData.RelativeLocation, createFolderData.Name)} }}");
                }

                return new CommandNone();
            }
            else if (command is CommandDeleteNonExistent commandDeleteNonExistent)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandDeleteNonExistent } ");

                var remoteTree = commandDeleteNonExistent.GetData();
                remoteTree.DeleteNonExistent(SharedFolderLocation);
                return new CommandNone();
            }

            if (Constants.Debug) Console.WriteLine("Server Processing Error { Found unsupported command }");

            return new CommandNone();
        }
    }
}