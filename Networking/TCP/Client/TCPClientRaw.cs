using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using Callback = Action<AsyncEventArgs>;

// Async TCP client that (dis)connects and sends/receives a stream of bytes
public class TCPClientRaw : ISubscriber
{
    readonly Socket Client;
    bool Connected = false;

    protected TCPClientRaw()
    {
        Client = new(SocketType.Stream, ProtocolType.Tcp);
    }

    protected TCPClientRaw(Socket client)
    {
        Client = client;
        Connected = true;
    }

    public void Connect(string ip, ushort port, Callback callback)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += (sender, args) =>
        {
            Connected = args.SocketError == SocketError.Success;
            callback(new AsyncEventArgs(args.SocketError, Connected));
        };

        if (!Client.ConnectAsync(args))
        {
            Connected = args.SocketError == SocketError.Success;
            callback(new AsyncEventArgs(args.SocketError, Connected));
        }
    }

    protected void SendAll(byte[] stream, Callback? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(new AsyncEventArgs(SocketError.NotConnected, 0));
            return;
        }

        SocketAsyncEventArgs args = new() { UserToken = callback };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.SendAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    protected void ReceiveAll(byte[] stream, Callback callback)
    {
        if (!Connected)
        {
            callback(new AsyncEventArgs(SocketError.NotConnected, 0));
            return;
        }

        SocketAsyncEventArgs args = new() { UserToken = callback };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.ReceiveAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    public void Disconnect(Callback? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(new AsyncEventArgs(SocketError.NotConnected));
            return;
        }

        SocketAsyncEventArgs args = new();
        if (callback != null)
        {
            args.Completed += (sender, args) =>
            {
                callback(new AsyncEventArgs(args.SocketError));
                Connected = false;
            };
        }

        if (!Client.DisconnectAsync(args))
        {
            callback?.Invoke(new AsyncEventArgs(args.SocketError));
            Connected = false;
        }
    }

    void OnSendReceiveShard(object? sender, SocketAsyncEventArgs args)
    {
        var bytesTransferredTotal = (uint)(args.Offset + args.BytesTransferred);
        if (args.SocketError != SocketError.Success || bytesTransferredTotal == args.Buffer?.Length)
        {
            if (args.LastOperation == SocketAsyncOperation.Send)
            {
                ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal, true));
            }
            else
            {
                // TODO: why on fail we give back the buffer
                args.SetBuffer(args.Buffer, 0, (int)bytesTransferredTotal);
                ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, args.Buffer, true));
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

        if (args.LastOperation == SocketAsyncOperation.Send)
        {
            ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal));

            if (!Client.SendAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
        else
        {
            ((Callback?)args.UserToken)?.Invoke(new AsyncEventArgs(args.SocketError, bytesTransferredTotal));

            if (!Client.ReceiveAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
    }

    public void Stop()
    {
        Client.Close();
    }
}
}
