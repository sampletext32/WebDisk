using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class HelloCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public HelloCommand(object data) : base(data)
        {
        }
    }
}