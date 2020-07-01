using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class GetFolderHtmlCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public GetFolderHtmlCommand(string data) : base(data)
        {
        }
    }
}