using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Entities.TreeNodes;

namespace Entities
{
    [Serializable]
    public abstract class SocketCommand
    {
        protected object Data { get; set; }

        public byte[] Serialize()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, this);
            byte[] data = ms.ToArray();
            return data;
        }

        public static SocketCommand Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            SocketCommand command = (SocketCommand) formatter.Deserialize(ms);
            return command;
        }

        protected SocketCommand(object data)
        {
            Data = data;
        }
    }

    [Serializable]
    public class EmptyCommand : SocketCommand
    {
        public EmptyCommand() : base(null)
        {
        }
    }


    [Serializable]
    public class HelloCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public HelloCommand(object data) : base(data)
        {
        }
    }

    [Serializable]
    public class GetFolderHtmlCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public GetFolderHtmlCommand(string data) : base(data)
        {
        }
    }

    [Serializable]
    public class CompareHashCommand : SocketCommand
    {
        public string GetData()
        {
            return (string) Data;
        }

        public CompareHashCommand(string data) : base(data)
        {
        }
    }

    [Serializable]
    public class ResponseCompareHashCommand : SocketCommand
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public ResponseCompareHashCommand(bool data) : base(data)
        {
        }
    }

    [Serializable]
    public class GetTreeCommand : SocketCommand
    {
        public GetTreeCommand() : base(null)
        {
        }
    }

    [Serializable]
    public class ResponseGetTreeCommand : SocketCommand
    {
        public TreeNode GetData()
        {
            return (TreeNode) Data;
        }

        public ResponseGetTreeCommand(TreeNode data) : base(data)
        {
        }
    }

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

    [Serializable]
    public class GetFileSizeCommand : SocketCommand
    {
        public FileSizeData GetData()
        {
            return (FileSizeData) Data;
        }

        public GetFileSizeCommand(FileSizeData fileSizeData) : base(fileSizeData)
        {
        }
    }

    [Serializable]
    public class ResponseGetFileSizeCommand : SocketCommand
    {
        public int GetData()
        {
            return (int) Data;
        }

        public ResponseGetFileSizeCommand(int size) : base(size)
        {
        }
    }

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

    [Serializable]
    public class GetFilePieceCommand : SocketCommand
    {
        public FilePieceDataLocation GetData()
        {
            return (FilePieceDataLocation) Data;
        }

        public GetFilePieceCommand(FilePieceDataLocation filePieceData) : base(filePieceData)
        {
        }
    }

    [Serializable]
    public class ResponseGetFilePieceCommand : SocketCommand
    {
        public byte[] GetData()
        {
            return (byte[]) Data;
        }

        public ResponseGetFilePieceCommand(byte[] piece) : base(piece)
        {
        }
    }

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

    [Serializable]
    public class UploadFilePieceCommand : SocketCommand
    {
        public FilePieceData GetData()
        {
            return (FilePieceData) Data;
        }

        public UploadFilePieceCommand(FilePieceData pieceData) : base(pieceData)
        {
        }
    }

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

    [Serializable]
    public class ResponseIsFilesEqualCommand : SocketCommand
    {
        public bool GetData()
        {
            return (bool) Data;
        }

        public ResponseIsFilesEqualCommand(bool differs) : base(differs)
        {
        }
    }
}