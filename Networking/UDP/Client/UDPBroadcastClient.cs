using Parser;
using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;

namespace Networking.UDP.Client
{
using Callback = Action<IPEndPoint>;

// Broadcasts every 5 seconds continously and
// listens for any ip with the udp server broadcast port and receives from any ip and port
public class UDPBroadcastClient
{
    readonly UDPBroadcastClientRaw client;
    Timer? timer = null;
    readonly ushort milliseconds;

    public UDPBroadcastClient(ushort port, byte seconds)
    {
        client = new(port);
        milliseconds = (ushort)(seconds * 1000);
    }

    public void Start(Callback callback)
    {
        timer = new Timer(
            _ => Broadcast(), null, 0, milliseconds);

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
                        BroadcastIPFinder.GetLocalIPV4().ToString() != endPointRemoteIP.Address.ToString())
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

        client.SendOrBroadcastToAll(messageBytes, new IPEndPoint(IPAddress.Parse(BroadcastIPFinder.GetBroadcastIP()),
                                                                 Utility.PORT_UDP_BROADCAST_SERVER));
    }
}
}
