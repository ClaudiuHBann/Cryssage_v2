using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using TCPClientRawCallbackConnect = Action<bool>;
using TCPClientRawCallbackSend = Action<uint, int>;
using TCPClientRawCallbackReceive = Action<uint, byte[]>;
using TCPClientRawCallbackDisconnect = Action<bool>;

public class TCPClientRaw
{
    Socket Client;

    public uint BufferReceiveSize { get; set; } = 8192;

    public TCPClientRaw()
    {
        Client = new(SocketType.Stream, ProtocolType.Tcp);
    }

    public void Connect(string ip, ushort port, TCPClientRawCallbackConnect? callback = null)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += (s, e) => callback?.Invoke(Client.Connected);

        Client.ConnectAsync(args);
    }

    public void Send(string ip, ushort port, TCPClientRawCallbackSend? callback = null)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += (s, e) => callback?.Invoke(Client.Connected);

        Client.SendAsync(args);
    }

    public void Receive(string ip, ushort port, TCPClientRawCallbackReceive? callback = null)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += (s, e) => callback?.Invoke(Client.Connected);

        Client.ReceiveAsync(args);
    }

    public void Disconnect(TCPClientRawCallbackDisconnect? callback = null)
    {
        SocketAsyncEventArgs args = new();
        args.Completed += (s, e) => callback?.Invoke(e.SocketError == SocketError.Success);

        Client.DisconnectAsync(args);
    }
}
}
