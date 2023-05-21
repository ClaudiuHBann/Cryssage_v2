using Parser;
using Parser.Message;

using Networking.Context;
using System.Net.Sockets;

namespace Networking.UDP.Client
{
public class UDPBroadcastClient : UDPBroadcastClientRaw
{
    public delegate void OnBroadcastDelegate(ContextProgress context);
    public static OnBroadcastDelegate OnBroadcast {
        get; set;
    } = new((_) =>
                {});

    public UDPBroadcastClient(ushort port) : base(port)
    {
    }

    public void Broadcast()
    {
        var message = MessageManager.ToMessage(Array.Empty<byte>(), Message.Type.BROADCAST);
        var messageBytes = MessageConverter.MessageToBytes(message);

        // TODO: watchout, the total bytes are the underlying message size
        var contextProgress = IContext.Create(message.PacketMetadata.Header.Type, message.PacketMetadata.Header.GUID,
                                              (uint)messageBytes.Length);

        BroadcastAll(messageBytes,
                     (args) =>
                     {
                         if (args.Error != SocketError.Success || args.Type != AsyncEventArgs.Type_.PROGRESS)
                         {
                             return;
                         }

                         contextProgress.SetPercentage(args.BytesTransferredTotal);
                         OnBroadcast(contextProgress);
                     });
    }
}
}
