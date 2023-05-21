using System.Net;
using System.Net.Sockets;

namespace Networking.UDP.Client
{
using Callback = Action<AsyncEventArgs>;

public class UDPBroadcastClientRaw
{
    readonly UdpClient Client = new();
    readonly ushort Port;

    protected UDPBroadcastClientRaw(ushort port)
    {
        Client.EnableBroadcast = true;
        Client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        Port = port;
    }

    protected void BroadcastAll(byte[] stream, Callback? callback = null)
    {
        SocketAsyncEventArgs args =
            new() { UserToken = callback, RemoteEndPoint = new IPEndPoint(IPAddress.Broadcast, Port) };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.Client.SendToAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    protected void ReceiveAll(byte[] stream, Callback callback)
    {
        SocketAsyncEventArgs args = new() { UserToken = callback, RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0) };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.Client.ReceiveFromAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    void OnSendReceiveShard(object? sender, SocketAsyncEventArgs args)
    {
        var bytesTransferredTotal = (uint)(args.Offset + args.BytesTransferred);
        if (args.SocketError != SocketError.Success || bytesTransferredTotal == args.Buffer?.Length)
        {
            if (args.LastOperation == SocketAsyncOperation.SendTo)
            {
                ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal, true));
            }
            else
            {
                // TODO: why on fail we give back the buffer
                args.SetBuffer(args.Buffer, 0, (int)bytesTransferredTotal);
                ((Callback?)args.UserToken)
                    ?.Invoke(new AsyncEventArgs(args.SocketError, args.Buffer, args.RemoteEndPoint, true));
            }

            return;
        }

        var offsetNew = (int)bytesTransferredTotal;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var countNew = args.Buffer.Length - offsetNew;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (countNew == 0)
        {
            return;
        }

        args.SetBuffer(args.Buffer, offsetNew, countNew);

        if (args.LastOperation == SocketAsyncOperation.SendTo)
        {
            ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal));

            if (!Client.Client.SendToAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
        else
        {
            ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal));

            if (!Client.Client.ReceiveFromAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
    }
}
}
