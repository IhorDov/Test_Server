using System.Net.Sockets;

namespace Client_part
{
    public interface ITcpClient
    {
        void Close();
        void Connect(string hostname, int port);
        INetworkStream GetStream();
    }

    public interface INetworkStream : IDisposable
    {
        StreamWriter GetWriter();
        StreamReader GetReader();
    }
}