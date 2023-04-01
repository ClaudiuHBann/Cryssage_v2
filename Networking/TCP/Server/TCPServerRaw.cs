using System.Net;
using System.Net.Sockets;

using Networking.TCP.Client;

namespace Networking.TCP.Server
{
using CallbackAccept = Action<SocketError, TCPClient>;

public class TCPServerRaw
{
    readonly Socket Server;

    public TCPServerRaw(ushort port)
    {
        Server = new(SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint endpoint = new(IPAddress.Any, port);
        Server.Bind(endpoint);
    }

    public void Start(CallbackAccept callback, int backlog = int.MaxValue)
    {
        Server.Listen(backlog);

        SocketAsyncEventArgs args = new() { UserToken = callback };
        args.Completed += CallbackAccept;

        if (!Server.AcceptAsync(args))
        {
            CallbackAccept(this, args);
        }
    }

    void CallbackAccept(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success && args.AcceptSocket != null)
        {
            ((CallbackAccept?)args.UserToken)?.Invoke(args.SocketError, new(args.AcceptSocket));
            args.AcceptSocket = null;
        }

        if (!Server.AcceptAsync(args))
        {
            CallbackAccept(this, args);
        }
    }

    public void Stop()
    {
        Server.Close();
    }
}
}
