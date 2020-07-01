using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandCreateFolder : Command
    {
        public FolderData GetData()
        {
            return (FolderData) Data;
        }

        public CommandCreateFolder(FolderData folderData) : base(folderData)
        {
        }
    }
}