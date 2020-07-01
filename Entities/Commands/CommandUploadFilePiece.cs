using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandUploadFilePiece : Command
    {
        public FilePieceData GetData()
        {
            return (FilePieceData) Data;
        }

        public CommandUploadFilePiece(FilePieceData pieceData) : base(pieceData)
        {
        }
    }
}