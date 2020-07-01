using System;
using Entities.DataObjects;

namespace Entities.Commands
{
    [Serializable]
    public class CommandIsFilesEqual : Command
    {
        public FileComparisonData GetData()
        {
            return (FileComparisonData) Data;
        }

        public CommandIsFilesEqual(FileComparisonData fileComparisonData) : base(fileComparisonData)
        {
        }
    }
}