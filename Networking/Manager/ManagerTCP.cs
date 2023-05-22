using Networking.TCP.Server;

namespace Networking.Manager
{
public class ManagerTCP
{
    public ManagerConnection ManagerConnection { get; }
    public ManagerTransferFile ManagerTransferFile { get; } = new();

    readonly TCPServerRaw server = new(Utility.PORT_TCP);
    readonly ServerDispatcher dispatcher;
    readonly ServerProcessor processor;

    public ManagerTCP(IContextHandler iContextHandler)
    {
        dispatcher = new(iContextHandler, ManagerTransferFile);
        processor = new(dispatcher);
        ManagerConnection = new(processor);

        server.Start(processor.Process);
    }
}
}
