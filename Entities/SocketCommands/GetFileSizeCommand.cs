using System;
using Entities.DataObjects;

namespace Entities.SocketCommands
{
    [Serializable]
    public class GetFileSizeCommand : SocketCommand
    {
        public FileSizeData GetData()
        {
            return (FileSizeData) Data;
        }

        public GetFileSizeCommand(FileSizeData fileSizeData) : base(fileSizeData)
        {
        }
    }
}