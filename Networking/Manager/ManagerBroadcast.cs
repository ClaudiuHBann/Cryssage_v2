using Parser.Message;

using Networking.Context;

using Networking.UDP.Client;
using Networking.UDP.Server;
using Networking.TCP.Server;

namespace Networking.Manager
{
public class ManagerBroadcast
{
    readonly UDPBroadcastServerRaw server;
    readonly UDPBroadcastClient client;
    readonly Timer timer;

    public ManagerBroadcast(ManagerConnection managerConnection)
    {
        UDPBroadcastClientRaw udpBroadcastRaw = new(Utility.PORT_UDP_BROADCAST);

        server = new(udpBroadcastRaw);
        client = new(udpBroadcastRaw);

        server.Start(ipEndPoint => managerConnection.CreateConnectionAndSend(
                         ipEndPoint.Address.ToString(), new ContextRequest(Message.Type.DISCOVER, Guid.NewGuid())));

        timer = new Timer(
            _ => client.Broadcast(), null, 0, 5000);
    }

    public void Broadcast() => client.Broadcast();
}
}
