using System.IO;

using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using TCPClientRawCallbackConnect = Action<SocketError, bool>;
using TCPClientRawCallbackSend = Action<SocketError, uint>;
using TCPClientRawCallbackReceive = Action<SocketError, byte[]?>;
using TCPClientRawCallbackDisconnect = Action<SocketError>;

class TCPClientRaw
{
    readonly Socket Client = new(SocketType.Stream, ProtocolType.Tcp);
    bool Connected = false;

    public void Connect(string ip, ushort port, TCPClientRawCallbackConnect? callback = null)
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

    public void SendAll(byte[] stream, TCPClientRawCallbackSend? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, 0);
            return;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, 0, stream.Length);
        if (callback != null)
        {
            args.Completed += (sender, args) => callback(args.SocketError, (uint)(args.Offset + args.BytesTransferred));
            args.UserToken = callback;
        }

        Client.SendAsync(args);
    }

    public void ReceiveAll(byte[] stream, TCPClientRawCallbackReceive? callback = null)
    {
        if (!Connected)
        {
            callback?.Invoke(SocketError.NotConnected, null);
            return;
        }

        SocketAsyncEventArgs args = new();
        args.SetBuffer(stream, 0, stream.Length);
        if (callback != null)
        {
            args.Completed += (sender, args) => callback(args.SocketError, args.Buffer);
            args.UserToken = callback;
        }

        Client.ReceiveAsync(args);
    }

    public void Disconnect(TCPClientRawCallbackDisconnect? callback = null)
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
                ((TCPClientRawCallbackSend?)args.UserToken)?.Invoke(args.SocketError, bytesTransferredTotal);
            }
            else
            {
                args.SetBuffer(args.Buffer, 0, (int)bytesTransferredTotal);
                ((TCPClientRawCallbackReceive?)args.UserToken)?.Invoke(args.SocketError, args.Buffer);
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
