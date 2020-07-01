using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandSimpleMessage : Command
    {
        public string GetData()
        {
            return (string) Data;
        }

        public CommandSimpleMessage(object data) : base(data)
        {
        }
    }
}