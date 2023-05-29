using Networking.Context.Interface;

namespace Networking.Manager
{
    public class ManagerNetwork
{
    readonly ManagerTCP ManagerTCP;
    readonly ManagerBroadcast ManagerBroadcast;

    public ManagerNetwork(IContextHandler iContextHandler)
    {
        ManagerTCP = new(iContextHandler);
        ManagerBroadcast = new(ManagerTCP.ManagerConnection);
    }

    public void Broadcast() => ManagerBroadcast.Broadcast();

    public void Send(string ip, IContext context) => ManagerTCP.ManagerConnection.Send(ip, context);
}
}
