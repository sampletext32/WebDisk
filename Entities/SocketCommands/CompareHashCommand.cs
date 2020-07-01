using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class CompareHashCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public CompareHashCommand(string data) : base(data)
        {
        }
    }
}