using System;

namespace Entities.DataObjects
{
    [Serializable]
    public class FileComparisonData
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }

        public FileComparisonData(string relativeLocation, string name, string hash)
        {
            RelativeLocation = relativeLocation;
            Name = name;
            Hash = hash;
        }
    }
}