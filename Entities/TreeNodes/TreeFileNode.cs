using System;
using System.IO;

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
            string hash;
            using (FileStream stream =
                new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open))
            {
                hash = CreateMD5(stream);
            }

            Hash = hash;
            return hash;
        }

        public override string WrapHtml(string rootLocation)
        {
            string hash = CalculateHash(rootLocation);
            string selfHtml = $"<li>{hash}</li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            string hash = CalculateHash(rootLocation);

            string selfHtml = $"<file>{hash}</file>";
            return selfHtml;
        }

        public override void BuildHierarchy(string rootLocation, bool ignoreRoot = true)
        {
            File.Create(Path.Combine(rootLocation, Name)).Close();
        }

        public override void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            var fileSizeData = new FileSizeData(this.RelativeLocation, Name);
            var getFileSizeCommand = new GetFileSizeCommand(fileSizeData);
            var getFileSizeCommandBytes = getFileSizeCommand.Serialize();
            var responseGetFileSizeCommandBytes = requestPerformer.PerformRequest(getFileSizeCommandBytes);
            var responseGetFileSizeCommand =
                (ResponseGetFileSizeCommand) SocketCommand.Deserialize(responseGetFileSizeCommandBytes);

            int fileSize = responseGetFileSizeCommand.GetData();

            int pieceSize = 16384;

            FileStream fs = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.CreateNew);

            int received = 0;
            while (received < fileSize)
            {
                int downloadSize = 0;
                if (fileSize - received < pieceSize)
                {
                    downloadSize = fileSize - received;
                }
                else
                {
                    downloadSize = pieceSize;
                }

                var getFilePieceCommand =
                    new GetFilePieceCommand(new FilePieceData(RelativeLocation, Name, received, downloadSize));
                var getFilePieceCommandBytes = getFilePieceCommand.Serialize();
                var responseGetFilePieceCommandBytes = requestPerformer.PerformRequest(getFilePieceCommandBytes);
                var responseGetFilePieceCommand =
                    (ResponseGetFilePieceCommand) SocketCommand.Deserialize(responseGetFilePieceCommandBytes);

                var pieceBytes = responseGetFilePieceCommand.GetData();

                fs.Write(pieceBytes, 0, downloadSize);

                received += downloadSize;
            }

            fs.Close();
        }

        public override void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }
    }
}