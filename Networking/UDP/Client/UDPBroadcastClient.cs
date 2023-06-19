using Parser;
using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;

namespace Networking.UDP.Client
{
using Callback = Action<IPEndPoint>;

// Listens for any ip with the udp server broadcast port and receives from any ip and port
public class UDPBroadcastClient
{
    readonly UDPBroadcastClientRaw client;
    readonly List<IPAddress> broadcastIPs = BroadcastIPFinder.GetBroadcastIPs();
    readonly List<IPAddress> localIPs = BroadcastIPFinder.GetLocalIPs();

    public UDPBroadcastClient(ushort port)
    {
        client = new(port);
    }

    public void Start(Callback callback)
    {
        Receive(callback);
    }

    void Receive(Callback callback)
    {
        // the broadcast message is just a metadata so thats all we need to read
        client.ReceiveFromAll(
            new byte[HeaderMetadata.SIZE], new IPEndPoint(IPAddress.Any, Utility.PORT_UDP_BROADCAST_SERVER),
            (args) =>
            {
                // no error and we got the metadata and the operation is done so we have the end point
                if (args.Error == SocketError.Success && args.Stream != null && args.Done)
                {
                    var metadata = MessageConverter.BytesToHeaderMetadata(args.Stream);
                    var endPointRemoteIP = (IPEndPoint?)args.EndPointRemote;

                    // we got a broadcast message and a valid remote end point and it's not us
                    if (metadata.Type == Message.Type.PING && endPointRemoteIP != null &&
                        !localIPs.Any(localIP => localIP.ToString() == endPointRemoteIP.Address.MapToIPv4().ToString()))
                    {
                        callback(endPointRemoteIP);
                    }
                }

                Receive(callback);
            });
    }

    public void Broadcast()
    {
        var message = MessageManager.ToMessage(Array.Empty<byte>(), Message.Type.PING);
        var messageBytes = MessageConverter.MessageToBytes(message);

        foreach (var broadcastIP in broadcastIPs)
        {
            client.SendOrBroadcastToAll(messageBytes, new IPEndPoint(broadcastIP, Utility.PORT_UDP_BROADCAST_SERVER));
        }
    }
}
}
