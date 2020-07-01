﻿using System;

namespace Entities
{
    [Serializable]
    public class FileSizeData
    {
        public string RelativeLocation { get; set; }
        public string Name { get; set; }

        public FileSizeData(string relativeLocation, string name)
        {
            RelativeLocation = relativeLocation;
            Name = name;
        }
    }
}