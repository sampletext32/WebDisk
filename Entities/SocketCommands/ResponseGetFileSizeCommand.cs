using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class ResponseGetFileSizeCommand : SocketCommand
    {
        public int GetData()
        {
            return (int) Data;
        }

        public ResponseGetFileSizeCommand(int size) : base(size)
        {
        }
    }
}