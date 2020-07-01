using System;

namespace Entities.DataObjects
{
    [Serializable]
    public class FolderData
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }

        public FolderData(string relativeLocation, string name)
        {
            RelativeLocation = relativeLocation;
            Name = name;
        }
    }
}