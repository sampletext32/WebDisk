using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandCompareHash : Command
    {
        public string GetData()
        {
            return (string) Data;
        }

        public CommandCompareHash(string data) : base(data)
        {
        }
    }
}