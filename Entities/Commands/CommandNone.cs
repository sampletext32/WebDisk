using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandNone : Command
    {
        public CommandNone() : base(null)
        {
        }
    }
}