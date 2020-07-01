using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class ResponseIsFilesEqualCommand : SocketCommand
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public ResponseIsFilesEqualCommand(bool differs) : base(differs)
        {
        }
    }
}