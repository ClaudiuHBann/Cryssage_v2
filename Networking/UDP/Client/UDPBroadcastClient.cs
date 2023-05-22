using Parser;
using Parser.Message;

namespace Networking.UDP.Client
{
public class UDPBroadcastClient
{
    readonly UDPBroadcastClientRaw client;

    public UDPBroadcastClient(UDPBroadcastClientRaw client)
    {
        this.client = client;
    }

    public void Broadcast()
    {
        Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

        var message = MessageManager.ToMessage(Array.Empty<byte>(), Message.Type.PING);
        var messageBytes = MessageConverter.MessageToBytes(message);

        client.BroadcastAll(messageBytes);
    }
}
}
