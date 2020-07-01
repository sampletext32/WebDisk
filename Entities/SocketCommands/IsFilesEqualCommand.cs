using System;

namespace Entities.SocketCommands
{
    [Serializable]
    public class IsFilesEqualCommand : SocketCommand
    {
        public FileComparationData GetData()
        {
            return (FileComparationData) Data;
        }

        public IsFilesEqualCommand(FileComparationData fileComparationData) : base(fileComparationData)
        {
        }
    }
}