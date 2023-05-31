using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;
using Networking.UDP.Client;

namespace Networking.UDP.Server
{
using Callback = Action<IPEndPoint>;

// Listens for any ip with the udp client broadcast port and receives from any ip and port
// and on receiving a ping message it sends it back to the sender
public class UDPBroadcastServerRaw
{
    readonly UDPBroadcastClientRaw server;

    public UDPBroadcastServerRaw(ushort port)
    {
        server = new(port);
    }

    public void Start(Callback callback)
    {
        // the broadcast message is just a metadata so thats all we need to read
        server.ReceiveFromAll(
            new byte[HeaderMetadata.SIZE], new IPEndPoint(IPAddress.Any, Utility.PORT_UDP_BROADCAST_CLIENT),
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
                        server.SendOrBroadcastToAll(args.Stream, endPointRemoteIP);
                    }
                }

                Start(callback);
            });
    }
}
}
