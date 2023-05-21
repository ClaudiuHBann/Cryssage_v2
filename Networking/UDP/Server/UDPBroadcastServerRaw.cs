using Parser.Message;
using Parser.Message.Header;

using System.Net;
using System.Net.Sockets;
using Networking.UDP.Client;
using System.Net.NetworkInformation;

namespace Networking.UDP.Server
{
public class UDPBroadcastServerRaw : UDPBroadcastClientRaw
{
    public delegate void OnReceiveDelegate(EndPoint endPoint);
    public static OnReceiveDelegate OnReceive {
        get; set;
    } = new((_) =>
                {});

    public UDPBroadcastServerRaw(ushort port) : base(port)
    {
    }

    // When we broadcast and listen we receive out broadcast so this function gets all the network interfaces we have
    // and checks if the broadcast message came from us
    static bool IsEndPointRemoteMe(EndPoint endPointRemote)
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(adapter => adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                              adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            .SelectMany(property => property.GetIPProperties().UnicastAddresses)
            .Where(addressUnicast => addressUnicast.Address.AddressFamily == AddressFamily.InterNetwork)
            .Any(addressUnicast => addressUnicast.Address == ((IPEndPoint)endPointRemote).Address);
    }

    public void Start()
    {
        // the broadcast message is just a metadata so thats all we need to read
        ReceiveAll(new byte[HeaderMetadata.SIZE],
                   (args) =>
                   {
                       // no error and we got the metadata and the operation is done so we have the end point
                       if (args.Error == SocketError.Success && args.Stream != null && args.Done)
                       {
                           var metadata = MessageConverter.BytesToHeaderMetadata(args.Stream);
                           // we got a broadcast message and a valid remote end point and it's not us
                           if (metadata.Type == Message.Type.BROADCAST && args.EndPointRemote != null &&
                               !IsEndPointRemoteMe(args.EndPointRemote))
                           {
                               OnReceive(args.EndPointRemote);
                           }
                       }

                       Start();
                   });
    }
}
}
