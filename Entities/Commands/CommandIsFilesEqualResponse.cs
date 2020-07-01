using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandIsFilesEqualResponse : Command
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public CommandIsFilesEqualResponse(bool differs) : base(differs)
        {
        }
    }
}