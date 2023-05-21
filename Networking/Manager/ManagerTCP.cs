using Networking.TCP.Server;

namespace Networking.Manager
{
    public class ManagerTCP
{
    public ManagerConnection ManagerConnection { get; } = new();

    readonly TCPServerRaw server;
    readonly ServerDispatcher dispatcher;
    readonly ServerProcessor processor;

    public ManagerTCP(IContextHandler iContextHandler)
    {
        server = new(Utility.PORT_TCP);
        dispatcher = new(iContextHandler);
        processor = new(dispatcher);

        server.Start(processor.Process);
    }
}
}
