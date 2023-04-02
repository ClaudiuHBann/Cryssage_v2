using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using CallbackConnect = Action<SocketError, bool>;
using CallbackSend = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, byte[]?>;
using CallbackDisconnect = Action<SocketError>;

public class TCPClientRaw
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

    public void Connect(string ip, ushort port, CallbackConnect? callback = null)
    {
        SocketAsyncEventArgs args = new() { RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port) };
        args.Completed += (sender, args) =>
        {
            Connected = args.SocketError == SocketError.Success;
            callback?.Invoke(args.SocketError, Connected);
        };

        if (!Client.ConnectAsync(args))
        {
            Connected = args.SocketError == SocketError.Success;
            callback?.Invoke(args.SocketError, Connected);
        }
    }

    protected void SendAll(byte[] stream, CallbackSend? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, 0);
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

    protected void ReceiveAll(byte[] stream, CallbackReceive? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, null);
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

        if (!Client.DisconnectAsync(args))
        {
            callback?.Invoke(args.SocketError);
        }
    }

    void OnSendReceiveShard(object? sender, SocketAsyncEventArgs args)
    {
        var bytesTransferredTotal = (uint)(args.Offset + args.BytesTransferred);
        if (args.SocketError != SocketError.Success || bytesTransferredTotal == args.Buffer?.Length)
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
            if (!Client.SendAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
        else
        {
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
