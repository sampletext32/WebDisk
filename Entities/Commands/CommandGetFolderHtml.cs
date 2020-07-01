using System;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetFolderHtml : Command
    {
        public string GetData()
        {
            return (string) Data;
        }

        public CommandGetFolderHtml(string data) : base(data)
        {
        }
    }
}