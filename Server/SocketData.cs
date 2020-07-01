using System.Net.Sockets;

namespace Server
{
    // объект состояния подключенного клиента
    public class SocketData
    {
        // количество записанных байт
        public int ReceivedBytes { get; set; }

        // сокет
        public Socket Socket { get; private set; }

        // буфер для получения данных
        public byte[] Buffer { get; private set; }

        public SocketData(Socket socket, int size)
        {
            Socket = socket;
            Buffer = new byte[size];
        }

        public void SetBufferSize(int size)
        {
            Buffer = new byte[size];
        }
    }
}