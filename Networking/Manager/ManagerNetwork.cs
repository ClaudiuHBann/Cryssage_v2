namespace Networking.Manager
{
public class ManagerNetwork
{
    public ManagerTCP ManagerTCP { get; set; }
    public ManagerBroadcast ManagerBroadcast { get; set; }

    public ManagerNetwork(IContextHandler iContextHandler)
    {
        ManagerTCP = new(iContextHandler);
        ManagerBroadcast = new(ManagerTCP.ManagerConnection);
    }
}
}
