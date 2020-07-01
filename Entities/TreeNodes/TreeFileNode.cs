using System;
using System.IO;
using Entities.DataObjects;
using Entities.Commands;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeFileNode : TreeNode
    {
        public TreeFileNode(string name) : base(name)
        {
        }

        public override string CalculateHash(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"CalculateHash File: {{ {this} }};");
            string hash;
            using (FileStream stream =
                new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open))
            {
                hash = Utils.CreateMD5(stream);
            }

            Hash = hash;
            return hash;
        }

        public override string WrapHtml(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHtml File: {{ {this} }};");
            string hash = CalculateHash(rootLocation);
            string selfHtml = $"<li>{Name} {hash}</li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHashedXML File: {{ {this} }};");
            string hash = CalculateHash(rootLocation);

            string selfHtml = $"<file>{Name} {hash}</file>";
            return selfHtml;
        }

        public override void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Download File: {{ {this} }};");

            var fileSizeData = new FileSizeData(RelativeLocation, Name);
            var getFileSizeCommand = new CommandGetFileSize(fileSizeData);
            var getFileSizeCommandBytes = getFileSizeCommand.Serialize();
            var responseGetFileSizeCommandBytes = requestPerformer.PerformRequest(getFileSizeCommandBytes);
            var responseGetFileSizeCommand =
                (CommandGetFileSizeResponse) Command.Deserialize(responseGetFileSizeCommandBytes);

            int fileSize = responseGetFileSizeCommand.GetData();
            FileStream fs = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.CreateNew);

            int received = 0;
            while (received != fileSize)
            {
                int downloadSize = 0;
                if (fileSize - received < Constants.SendingFilePieceSize)
                {
                    downloadSize = fileSize - received;
                }
                else
                {
                    downloadSize = Constants.SendingFilePieceSize;
                }

                var getFilePieceCommand =
                    new CommandGetFilePiece(new FilePieceDataLocation(RelativeLocation, Name, received, downloadSize));
                var getFilePieceCommandBytes = getFilePieceCommand.Serialize();
                var responseGetFilePieceCommandBytes = requestPerformer.PerformRequest(getFilePieceCommandBytes);
                var responseGetFilePieceCommand =
                    (CommandGetFilePieceResponse) Command.Deserialize(responseGetFilePieceCommandBytes);

                var pieceBytes = responseGetFilePieceCommand.GetData();

                fs.Write(pieceBytes, 0, downloadSize);

                received += downloadSize;
            }

            fs.Close();
        }

        public override void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Upload File: {{ {this} }};");

            var fileComparisonData = new FileComparisonData(RelativeLocation, Name, Hash);
            var isFileDiffersCommand = new CommandIsFilesEqual(fileComparisonData);
            var isFileDiffersCommandBytes = isFileDiffersCommand.Serialize();
            var responseIsFileDiffersCommandBytes = requestPerformer.PerformRequest(isFileDiffersCommandBytes);
            var responseIsFilesEqualCommand =
                (CommandIsFilesEqualResponse) Command.Deserialize(responseIsFileDiffersCommandBytes);
            if (!responseIsFilesEqualCommand.GetData())
            {
                Console.WriteLine($"Performing upload {Name}");
                // Файлы отличаются
                FileStream fs = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open);
                int fileSize = (int) fs.Length; // длина файла

                byte[] buffer = new byte[Constants.SendingFilePieceSize];

                int sent = 0;
                while (sent != fileSize)
                {
                    int uploadSize = 0;
                    if (fileSize - sent < Constants.SendingFilePieceSize)
                    {
                        uploadSize = fileSize - sent;
                        buffer = new byte[uploadSize];
                    }
                    else
                    {
                        uploadSize = pieceSize;
                    }

                    fs.Read(buffer, 0, uploadSize);

                    var uploadFilePieceCommand =
                        new CommandUploadFilePiece(new FilePieceData(RelativeLocation, Name, sent, uploadSize, buffer));
                    var uploadFilePieceCommandBytes = uploadFilePieceCommand.Serialize();
                    var responseUploadFilePieceCommandBytes =
                        requestPerformer.PerformRequest(uploadFilePieceCommandBytes);
                    // ignore response, it's empty

                    sent += uploadSize;
                }

                fs.Close();
            }
            else
            {
                Console.WriteLine($"File is in sync {this}");
            }
        }
    }
}