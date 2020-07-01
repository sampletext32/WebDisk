using System;

namespace Entities.DataObjects
{
    [Serializable]
    public class FilePieceDataLocation
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }

        public FilePieceDataLocation(string relativeLocation, string name, int offset, int size)
        {
            RelativeLocation = relativeLocation;
            Name = name;
            Offset = offset;
            Size = size;
        }
    }
}