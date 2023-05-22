using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;
using Networking.UDP.Client;

namespace Networking.UDP.Server
{
using Callback = Action<IPEndPoint>;

public class UDPBroadcastServerRaw
{
    readonly UDPBroadcastClientRaw client;

    public UDPBroadcastServerRaw(UDPBroadcastClientRaw client)
    {
        this.client = client;
    }

    public void Start(Callback callback)
    {
        // the broadcast message is just a metadata so thats all we need to read
        client.ReceiveAll(new byte[HeaderMetadata.SIZE],
                          (args) =>
                          {
                              // no error and we got the metadata and the operation is done so we have the end point
                              if (args.Error == SocketError.Success && args.Stream != null && args.Done)
                              {
                                  var metadata = MessageConverter.BytesToHeaderMetadata(args.Stream);
                                  var endPointRemoteIP = (IPEndPoint?)args.EndPointRemote;

                                  // we got a broadcast message and a valid remote end point and it's not us
                                  if (metadata.Type == Message.Type.PING && endPointRemoteIP != null &&
                                      BroadcastIPFinder.GetLocalIPV4().ToString() !=
                                          endPointRemoteIP.Address.ToString())
                                  {
                                      callback(endPointRemoteIP);
                                  }
                              }

                              Start(callback);
                          });
    }
}
}
