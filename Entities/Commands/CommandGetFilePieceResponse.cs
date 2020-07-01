using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetFilePieceResponse : Command
    {
        public byte[] GetData()
        {
            return (byte[]) Data;
        }

        public CommandGetFilePieceResponse(byte[] piece) : base(piece)
        {
        }
    }
}