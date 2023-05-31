using System.Net;
using System.Net.Sockets;

namespace Networking.UDP.Client
{
using Callback = Action<AsyncEventArgs>;

public class UDPBroadcastClientRaw
{
    readonly UdpClient Client = new();

    public UDPBroadcastClientRaw(ushort port)
    {
        Client.EnableBroadcast = true;
        Client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
    }

    public void SendOrBroadcastToAll(byte[] stream, IPEndPoint endPointRemote, Callback? callback = null)
    {
        SocketAsyncEventArgs args = new() { UserToken = callback, RemoteEndPoint = endPointRemote };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.Client.SendToAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    public void ReceiveFromAll(byte[] stream, IPEndPoint endPointRemote, Callback callback)
    {
        SocketAsyncEventArgs args = new() { UserToken = callback, RemoteEndPoint = endPointRemote };
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
