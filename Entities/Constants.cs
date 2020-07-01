namespace Entities
{
    public class Constants
    {
        public const int SendingFilePieceSize = 1024768; // 1 Mb
        public const int ConnectionPort = 11771; // Default connection port
        public const int ConnectionTimeoutMilliseconds = 100; // Default client connection timeout
        public const int ServerMaximumBacklogSockets = 100; // Default waiting sockets amount
    }
}
