using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandCompareHashResponse : Command
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public CommandCompareHashResponse(bool data) : base(data)
        {
        }
    }
}