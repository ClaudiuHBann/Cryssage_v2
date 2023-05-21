using Parser;
using Parser.Message;

namespace Networking.UDP.Client
{
public class UDPBroadcastClient : UDPBroadcastClientRaw
{
    public UDPBroadcastClient(ushort port) : base(port)
    {
    }

    public void Broadcast()
    {
        var message = MessageManager.ToMessage(Array.Empty<byte>(), Message.Type.BROADCAST);
        var messageBytes = MessageConverter.MessageToBytes(message);

        BroadcastAll(messageBytes);
    }
}
}
