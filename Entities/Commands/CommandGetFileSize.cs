using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetFileSize : Command
    {
        public FileSizeData GetData()
        {
            return (FileSizeData) Data;
        }

        public CommandGetFileSize(FileSizeData fileSizeData) : base(fileSizeData)
        {
        }
    }
}