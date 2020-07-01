using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetFileSizeResponse : Command
    {
        public int GetData()
        {
            return (int) Data;
        }

        public CommandGetFileSizeResponse(int size) : base(size)
        {
        }
    }
}