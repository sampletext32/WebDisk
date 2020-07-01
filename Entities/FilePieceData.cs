using System;

namespace Entities
{
    [Serializable]
    public class FilePieceData
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }

        public FilePieceData(string relativeLocation, string name, int offset, int size, byte[] bytes)
        {
            RelativeLocation = relativeLocation;
            Name = name;
            Offset = offset;
            Size = size;
            Data = bytes;
        }
    }
}