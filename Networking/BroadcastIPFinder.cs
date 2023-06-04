using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Networking
{
public class BroadcastIPFinder
{
    public static string GetBroadcastIP()
    {
        var localIPV4 = GetLocalIPV4();
        var addressUnicast =
            NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(property => property.GetIPProperties().UnicastAddresses)
                .First(addressUnicast => addressUnicast.Address.AddressFamily == AddressFamily.InterNetwork &&
                                         localIPV4.ToString() == addressUnicast.Address.MapToIPv4().ToString());

        return GetBroadcastAddress(addressUnicast).ToString();
    }

    public static IPAddress GetLocalIPV4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(addr => addr.AddressFamily == AddressFamily.InterNetwork);
    }

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
