using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Entities.Commands
{
    [Serializable]
    public abstract class Command
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

        public static Command Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            Command command = (Command) formatter.Deserialize(ms);
            return command;
        }

        protected Command(object data)
        {
            Data = data;
        }
    }
}