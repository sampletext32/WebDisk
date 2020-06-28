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
}