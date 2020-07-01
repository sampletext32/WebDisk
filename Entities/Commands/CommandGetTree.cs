using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetTree : Command
    {
        public CommandGetTree() : base(null)
        {
        }
    }
}