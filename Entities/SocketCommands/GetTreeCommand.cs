using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class GetTreeCommand : SocketCommand
    {
        public GetTreeCommand() : base(null)
        {
        }
    }
}