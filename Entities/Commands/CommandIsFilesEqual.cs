using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandIsFilesEqual : Command
    {
        public FileComparationData GetData()
        {
            return (FileComparationData) Data;
        }

        public CommandIsFilesEqual(FileComparationData fileComparationData) : base(fileComparationData)
        {
        }
    }
}