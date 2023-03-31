using System.IO;

using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
public class TCPClientRaw
{
    protected Socket Client = new(SocketType.Stream, ProtocolType.Tcp);

    protected Action<SocketError, bool>? CallbackConnect = null;
    protected Action<SocketError, uint>? CallbackSend = null;
    protected Action<SocketError, byte[]?>? CallbackReceive = null;
    protected Action<SocketError>? CallbackDisconnect = null;

    public bool Connect(string ip, ushort port)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += OnConnect;

        return Client.ConnectAsync(args);
    }

    public void SendAll(byte[] stream)
    {
        if (Client == null)
        {
            CallbackSend?.Invoke(SocketError.NotConnected, 0);
            return;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        Client.SendAsync(args);
    }

    public bool ReceiveAll(byte[] stream, uint offset, uint count)
    {
        if (Client == null)
        {
            return false;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, (int)offset, (int)count);
        args.Completed += OnSendReceiveShard;

        var succeeded = Client?.ReceiveAsync(args);
        return succeeded != null && (bool)succeeded;
    }

    public bool Disconnect()
    {
        if (Client == null)
        {
            CallbackDisconnect?.Invoke(SocketError.NotConnected);
            return false;
        }

        SocketAsyncEventArgs args = new();
        args.Completed += OnDisconnect;

        return Client.DisconnectAsync(args);
    }

    void OnConnect(object? sender, SocketAsyncEventArgs args)
    {
        CallbackConnect?.Invoke(args.SocketError, args.SocketError == SocketError.Success);
    }

    void OnSendReceiveShard(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError != SocketError.Success || args.Offset + args.BytesTransferred == args.Buffer?.Length)
        {
            CallbackSend?.Invoke(args.SocketError, (uint)(args.Offset + args.BytesTransferred));
        }

        var offsetNew = args.Offset + args.BytesTransferred;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var countNew = args.Buffer.Length - offsetNew;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        args.SetBuffer(args.Buffer, offsetNew, countNew);

        if (args.LastOperation == SocketAsyncOperation.Send)
        {
            Client?.SendAsync(args);
        }
        else
        {
            Client?.ReceiveAsync(args);
        }
    }

    void OnDisconnect(object? sender, SocketAsyncEventArgs args)
    {
        CallbackDisconnect?.Invoke(args.SocketError);
    }
}
}
