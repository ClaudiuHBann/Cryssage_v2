using Networking.Context;

using Networking.TCP.Client;
using Networking.TCP.Server;

namespace Networking.Manager
{
public class ManagerTCP
{
    readonly ManagerFileTransfer managerFileTransfer;

    public ManagerConnection ManagerConnection { get; }
    readonly ClientDispatcher dispatcherClient;
    readonly ClientProcessor processorClient;

    readonly TCPServerRaw server = new(Utility.PORT_TCP);
    readonly ServerDispatcher dispatcherServer;
    readonly ServerProcessor processorServer;

    public ManagerTCP(IContextHandler iContextHandler, List<ContextFileInfo> contextFileInfos)
    {
        managerFileTransfer = new(iContextHandler, contextFileInfos);

        dispatcherClient = new(iContextHandler, managerFileTransfer);
        processorClient = new(dispatcherClient);
        ManagerConnection = new(processorClient);

        dispatcherServer = new(iContextHandler, managerFileTransfer, ManagerConnection);
        processorServer = new(dispatcherServer);

        server.Start(processorServer.Process);
    }
}
}
