﻿using System;
using Entities.DataObjects;

namespace Entities.SocketCommands
{
    [Serializable]
    public class CreateFolderCommand : SocketCommand
    {
        public FolderData GetData()
        {
            return (FolderData) Data;
        }

        public CreateFolderCommand(FolderData folderData) : base(folderData)
        {
        }
    }
}