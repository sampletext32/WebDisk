using System;
using Entities.DataObjects;

namespace Entities.SocketCommands
{
    [Serializable]
    public class UploadFilePieceCommand : SocketCommand
    {
        public FilePieceData GetData()
        {
            return (FilePieceData) Data;
        }

        public UploadFilePieceCommand(FilePieceData pieceData) : base(pieceData)
        {
        }
    }
}