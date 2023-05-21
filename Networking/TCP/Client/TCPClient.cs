using Parser;
using Parser.Message;
using Parser.Message.Header;

using System.Net.Sockets;

using Networking.Context;

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

    static void SendCallback(Callback callback, CallbackProgress callbackProgress, ContextProgress context,
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
            callbackProgress(context);
        }
    }

    public void Send(IContext context, Callback callback, CallbackProgress callbackProgress)
    {
        var contextAsBytes = context.ToStream();

        // create the message that will be sent
        var message = MessageManager.ToMessage(contextAsBytes, context.Type, context.GUID);
        var messageBytes = MessageConverter.MessageToBytes(message);

        // create the context progress
        // TODO: watchout, the total bytes are the underlying message size
        var contextProgress = IContext.Create(context.Type, context.GUID, (uint)messageBytes.Length);

        SendAll(messageBytes, (args) => SendCallback(callback, callbackProgress, contextProgress, args));
    }

    static MessageDisassembled CreateMessageDisassembledFromMetadataAndData(byte[] metadataAsBytes, byte[] dataAsBytes)
    {
        var bytes = new byte[metadataAsBytes.Length + dataAsBytes.Length];
        metadataAsBytes.CopyTo(bytes, 0);
        dataAsBytes.CopyTo(bytes, metadataAsBytes.Length);

        var message = MessageConverter.BytesToMessage(bytes);
        return MessageManager.FromMessage(message);
    }

    // when the operation is done the context is the data context else the context is the progress one
    static void ReceiveCallback(Callback callback, IContext context, AsyncEventArgs args)
    {
        if (args.Type != AsyncEventArgs.Type_.PROGRESS || args.Type != AsyncEventArgs.Type_.RECEIVE)
        {
            return;
        }

        if (context.Type == Message.Type.PROGRESS)
        {
            ((ContextProgress)context).SetPercentage(args.BytesTransferredTotal);
        }

        callback(context);
    }

    static void ReceiveCallback(Callback callback, CallbackProgress callbackProgress, ContextProgress contextProgress,
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
            callbackProgress(contextProgress);
        }
    }

    public void Receive(Callback callback, CallbackProgress callbackProgress)
    {
        ReceiveAll(new byte[HeaderMetadata.SIZE],
                   (argsMetadata) =>
                   {
                       if (argsMetadata.Error != SocketError.Success || argsMetadata.Stream == null)
                       {
                           return;
                       }

                       var metadata = MessageConverter.BytesToHeaderMetadata(argsMetadata.Stream);
                       // TODO: watchout, the total bytes are the underlying message size
                       var contextProgress = IContext.Create(metadata.Type, metadata.GUID, metadata.Size);
                       ReceiveAll(new byte[metadata.Size],
                                  (argsData) => ReceiveCallback(callback, callbackProgress, contextProgress,
                                                                argsMetadata.Stream, argsData));
                   });
    }
}
}
