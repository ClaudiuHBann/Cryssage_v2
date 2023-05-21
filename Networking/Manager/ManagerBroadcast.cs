using Networking.UDP.Client;
using Networking.UDP.Server;

namespace Networking.Manager
{
public class ManagerBroadcast
{
    readonly UDPBroadcastServerRaw server = new(Utility.PORT_UDP_BROADCAST);
    readonly UDPBroadcastClient client = new(Utility.PORT_UDP_BROADCAST);

    public ManagerBroadcast(ManagerConnection managerConnection)
    {
        server.Start(managerConnection.CreateConnection);
    }

    public void Broadcast() => client.Broadcast();
}
}
