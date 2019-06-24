using System.Net.Sockets;

namespace HttpService
{
    public class StateModel
    {
        public Socket Connection { get; }
        public byte[] Buffer { get; }

        public StateModel(Socket socket, byte[] buffer)
        {
            Connection = socket;
            Buffer = buffer;
        }
    }
}
