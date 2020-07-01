using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class EmptyCommand : SocketCommand
    {
        public EmptyCommand() : base(null)
        {
        }
    }
}