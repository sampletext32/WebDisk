using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Entities.SocketCommands
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
}