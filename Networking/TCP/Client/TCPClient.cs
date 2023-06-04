using Parser;
using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;

using Networking.Context;
using Networking.Context.Interface;

namespace Networking.TCP.Client
{
    using Callback = Action<IContext>;
    using CallbackProgress = Action<ContextProgress>;

    // TCPClientRaw that sends/receives messages
    public class TCPClient : TCPClientRaw
{
    public TCPClient()
    {
    }

    public TCPClient(Socket socket) : base(socket)
    {
    }

    public IPEndPoint? EndPointLocal => (IPEndPoint?)Client.Client.LocalEndPoint;
    public IPEndPoint? EndPointRemote => (IPEndPoint?)Client.Client.RemoteEndPoint;
    public bool Connected => Client.Client.Connected;

    static void SendCallback(CallbackProgress callback, CallbackProgress? callbackProgress, ContextProgress context,
                             AsyncEventArgs args)
    {
        if (args.Error != SocketError.Success || args.Type != AsyncEventArgs.Type_.PROGRESS)
        {
            return;
        }

        context.SetPercentage(args.BytesTransferredTotal);
        if (context.Done)
        {
            callback(context);
        }
        else
        {
            callbackProgress?.Invoke(context);
        }
    }

    public void Send(IContext context, CallbackProgress callback, CallbackProgress? callbackProgress = null)
    {
        var contextAsBytes = context.ToStream();

        // create the message that will be sent
        var message = MessageManager.ToMessage(contextAsBytes, context.Type, context.GUID);
        var messageBytes = MessageConverter.MessageToBytes(message);

        // create the context progress
        var contextProgress = new ContextProgress(ContextProgress.Type_.SEND, (uint)messageBytes.Length, context.GUID);
        SendAll(messageBytes, (args) => SendCallback(callback, callbackProgress, contextProgress, args));
    }

    static MessageDisassembled CreateMessageDisassembledFromMetadataAndData(byte[] metadataAsBytes, byte[] dataAsBytes)
    {
        var bytes = new byte[metadataAsBytes.Length + dataAsBytes.Length];
        metadataAsBytes.CopyTo(bytes, 0);
        dataAsBytes.CopyTo(bytes, metadataAsBytes.Length);

        var packetMetadata = MessageConverter.BytesToPacketMetadata(metadataAsBytes);
        var message = MessageConverter.BytesToMessage(bytes, packetMetadata.Header.Fragmented);
        return MessageManager.FromMessage(message, packetMetadata.Header.Fragmented);
    }

    static void ReceiveCallback(Callback callback, CallbackProgress? callbackProgress, ContextProgress contextProgress,
                                byte[] metadataAsBytes, AsyncEventArgs argsData)
    {
        // error or no data for data context when operation finished
        if (argsData.Error != SocketError.Success || (argsData.Done && argsData.Stream == null))
        {
            return;
        }

        // operation is done, create the data context else send progress context
        if (argsData.Done && argsData.Stream != null)
        {
            var messageDisassembled = CreateMessageDisassembledFromMetadataAndData(metadataAsBytes, argsData.Stream);
            if (messageDisassembled.Stream == null)
            {
                return;
            }

            var context =
                IContext.Create(messageDisassembled.Type, messageDisassembled.GUID, messageDisassembled.Stream);
            if (context == null)
            {
                return;
            }

            callback(context);
        }
        else
        {
            contextProgress.SetPercentage(argsData.BytesTransferredTotal);
            callbackProgress?.Invoke(contextProgress);
        }
    }

    public void Receive(Callback callback, CallbackProgress? callbackProgress = null)
    {
        ReceiveAll(new byte[HeaderMetadata.SIZE],
                   argsMetadata =>
                   {
                       if (argsMetadata.Error != SocketError.Success || argsMetadata.Stream == null)
                       {
                           return;
                       }

                       var metadata = MessageConverter.BytesToHeaderMetadata(argsMetadata.Stream);
                       var contextProgress =
                           new ContextProgress(ContextProgress.Type_.RECEIVE, metadata.Size, metadata.GUID);
                       ReceiveAll(new byte[metadata.Size],
                                  argsData => ReceiveCallback(callback, callbackProgress, contextProgress,
                                                              argsMetadata.Stream, argsData));
                   });
    }
}
}
