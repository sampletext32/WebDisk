using System;

namespace Entities.DataObjects
{
    [Serializable]
    public class FileComparationData
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }

        public FileComparationData(string relativeLocation, string name, string hash)
        {
            RelativeLocation = relativeLocation;
            Name = name;
            Hash = hash;
        }
    }
}