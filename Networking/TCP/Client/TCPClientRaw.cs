using System.Net;
using System.Net.Sockets;

namespace Networking.TCP.Client
{
using CallbackConnect = Action<SocketError, bool>;
using CallbackSend = Action<SocketError, uint>;
using CallbackSendShard = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, byte[]?>;
using CallbackReceiveShard = Action<SocketError, byte[]?>;
using CallbackDisconnect = Action<SocketError>;

public class TCPClientRaw
{
    class SAEAUserTokenSend
    {
        public CallbackSend? CallbackSend { get; set; } = null;
        public CallbackSendShard? CallbackSendShard { get; set; } = null;

        public SAEAUserTokenSend(CallbackSend? callbackSend = null, CallbackSendShard? callbackSendShard = null)
        {
            CallbackSend = callbackSend;
            CallbackSendShard = callbackSendShard;
        }
    }

    class SAEAUserTokenReceive
    {
        public CallbackReceive? CallbackReceive { get; set; } = null;
        public CallbackReceiveShard? CallbackReceiveShard { get; set; } = null;

        public SAEAUserTokenReceive(CallbackReceive? callbackReceive = null,
                                    CallbackReceiveShard? callbackReceiveShard = null)
        {
            CallbackReceive = callbackReceive;
            CallbackReceiveShard = callbackReceiveShard;
        }
    }

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

    protected void SendAll(byte[] stream, CallbackSend? callbackSend = null,
                           CallbackSendShard? callbackSendShard = null)
    {
        if (!Connected)
        {
            callbackSend?.Invoke(SocketError.NotConnected, 0);
            callbackSendShard?.Invoke(SocketError.NotConnected, 0);
            return;
        }

        SocketAsyncEventArgs args = new() { UserToken = new SAEAUserTokenSend(callbackSend, callbackSendShard) };
        args.SetBuffer(stream, 0, stream.Length);
        args.Completed += OnSendReceiveShard;

        if (!Client.SendAsync(args))
        {
            OnSendReceiveShard(this, args);
        }
    }

    protected void ReceiveAll(byte[] stream, CallbackReceive? callbackReceive = null,
                              CallbackReceiveShard? callbackReceiveShard = null)
    {
        if (!Connected)
        {
            callbackReceive?.Invoke(SocketError.NotConnected, null);
            callbackReceiveShard?.Invoke(SocketError.NotConnected, null);
            return;
        }

        SocketAsyncEventArgs args =
            new() { UserToken = new SAEAUserTokenReceive(callbackReceive, callbackReceiveShard) };
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
                var userTokenSend = (SAEAUserTokenSend?)args.UserToken;
                userTokenSend?.CallbackSend?.Invoke(args.SocketError, bytesTransferredTotal);
            }
            else
            {
                var userTokenReceive = (SAEAUserTokenReceive?)args.UserToken;
                args.SetBuffer(args.Buffer, 0, (int)bytesTransferredTotal);
                userTokenReceive?.CallbackReceive?.Invoke(args.SocketError, args.Buffer);
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

        var userTokenReceiveShardBufferRange = args.Offset..offsetNew;
        args.SetBuffer(args.Buffer, offsetNew, countNew);

        if (args.LastOperation == SocketAsyncOperation.Send)
        {
            var userTokenSend = (SAEAUserTokenSend?)args.UserToken;
            userTokenSend?.CallbackSendShard?.Invoke(args.SocketError, bytesTransferredTotal);

            if (!Client.SendAsync(args))
            {
                OnSendReceiveShard(this, args);
            }
        }
        else
        {
            var userTokenReceive = (SAEAUserTokenReceive?)args.UserToken;
            userTokenReceive?.CallbackReceiveShard?.Invoke(args.SocketError,
                                                           args.Buffer[userTokenReceiveShardBufferRange]);

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
