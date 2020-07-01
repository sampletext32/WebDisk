using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetFilePiece : Command
    {
        public FilePieceDataLocation GetData()
        {
            return (FilePieceDataLocation) Data;
        }

        public CommandGetFilePiece(FilePieceDataLocation filePieceData) : base(filePieceData)
        {
        }
    }
}