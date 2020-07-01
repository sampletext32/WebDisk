using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class ResponseGetFilePieceCommand : SocketCommand
    {
        public byte[] GetData()
        {
            return (byte[]) Data;
        }

        public ResponseGetFilePieceCommand(byte[] piece) : base(piece)
        {
        }
    }
}