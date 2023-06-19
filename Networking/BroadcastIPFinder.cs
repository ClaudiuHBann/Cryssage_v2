using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Networking
{
public class BroadcastIPFinder
{
    public static List<IPAddress> GetBroadcastIPs()
    {
        var broadcastIPs =
            NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(property => property.GetIPProperties().UnicastAddresses)
                .Where(addressUnicast => addressUnicast.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(GetBroadcastAddress)
                .ToList();
        broadcastIPs.Add(IPAddress.Broadcast);

        return broadcastIPs;
    }

    public static List<IPAddress> GetLocalIPs() =>
        NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(property => property.GetIPProperties().UnicastAddresses)
            .Where(addressUnicast => addressUnicast.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(addressUnicast => addressUnicast.Address)
            .ToList();

    static IPAddress GetBroadcastAddress(UnicastIPAddressInformation unicastAddress) =>
        GetBroadcastAddress(unicastAddress.Address, unicastAddress.IPv4Mask);

    static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
    {
        var ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
        var ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
        var broadCastIpAddress = ipAddress | ~ipMaskV4;

        return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
    }
}
}
