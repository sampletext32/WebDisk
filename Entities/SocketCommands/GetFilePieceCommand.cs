using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class GetFilePieceCommand : SocketCommand
    {
        public FilePieceDataLocation GetData()
        {
            return (FilePieceDataLocation) Data;
        }

        public GetFilePieceCommand(FilePieceDataLocation filePieceData) : base(filePieceData)
        {
        }
    }
}