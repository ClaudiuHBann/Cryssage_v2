using Networking.UDP.Client;
using Networking.UDP.Server;

using Networking.Context.Request;

namespace Networking.Manager
{
public class ManagerBroadcast
{
    readonly UDPBroadcastServerRaw server = new(Utility.PORT_UDP_BROADCAST_SERVER);
    readonly UDPBroadcastClient client = new(Utility.PORT_UDP_BROADCAST_CLIENT, 5);

    public ManagerBroadcast(ManagerConnection managerConnection)
    {
        server.Start(ipEndPoint => managerConnection.Send(ipEndPoint.Address.ToString(), new ContextDiscoverRequest()));
        client.Start(ipEndPoint => managerConnection.Send(ipEndPoint.Address.ToString(), new ContextDiscoverRequest()));
    }

    public void Broadcast() => client.Broadcast();
}
}
