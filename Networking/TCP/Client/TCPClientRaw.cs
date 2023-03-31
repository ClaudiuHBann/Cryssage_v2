using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using CallbackConnect = Action<SocketError, bool>;
using CallbackSend = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, byte[]?>;
using CallbackDisconnect = Action<SocketError>;

class TCPClientRaw
{
    protected readonly Socket Client = new(SocketType.Stream, ProtocolType.Tcp);
    protected bool Connected = false;

    public void Connect(string ip, ushort port, CallbackConnect? callback = null)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        if (callback != null)
        {
            args.Completed += (sender, args) =>
            {
                Connected = args.SocketError == SocketError.Success;
                callback(args.SocketError, Connected);
            };
        }

        Client.ConnectAsync(args);
    }

    protected void SendAll(byte[] stream, CallbackSend? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, 0);
            return;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, 0, stream.Length);
        args.UserToken = callback;
        args.Completed += OnSendReceiveShard;

        Client.SendAsync(args);
    }

    protected void ReceiveAll(byte[] stream, CallbackReceive? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, null);
            return;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, 0, stream.Length);
        args.UserToken = callback;
        args.Completed += OnSendReceiveShard;

        Client.ReceiveAsync(args);
    }

    public void Disconnect(CallbackDisconnect? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected);
            return;
        }

        SocketAsyncEventArgs args = new();
        if (callback != null)
        {
            args.Completed += (sender, args) => callback(args.SocketError);
        }

        Client.DisconnectAsync(args);
    }

    void OnSendReceiveShard(object? sender, SocketAsyncEventArgs args)
    {
        var bytesTransferredTotal = (uint)(args.Offset + args.BytesTransferred);
        if (args.SocketError != SocketError.Success || args.Offset + args.BytesTransferred == args.Buffer?.Length)
        {
            if (args.LastOperation == SocketAsyncOperation.Send)
            {
                ((CallbackSend?)args.UserToken)?.Invoke(args.SocketError, bytesTransferredTotal);
            }
            else
            {
                args.SetBuffer(args.Buffer, 0, (int)bytesTransferredTotal);
                ((CallbackReceive?)args.UserToken)?.Invoke(args.SocketError, args.Buffer);
            }
        }

        var offsetNew = (int)bytesTransferredTotal;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var countNew = args.Buffer.Length - offsetNew;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        args.SetBuffer(args.Buffer, offsetNew, countNew);

        if (args.LastOperation == SocketAsyncOperation.Send)
        {
            Client.SendAsync(args);
        }
        else
        {
            Client.ReceiveAsync(args);
        }
    }
}
}
