using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class ResponseCompareHashCommand : SocketCommand
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public ResponseCompareHashCommand(bool data) : base(data)
        {
        }
    }
}