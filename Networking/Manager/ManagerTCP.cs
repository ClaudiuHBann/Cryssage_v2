using Networking.TCP.Server;

namespace Networking.Manager
{
    public class ManagerTCP
{
    public ManagerConnection ManagerConnection { get; } = new();

    readonly TCPServerRaw server = new(Utility.PORT_TCP);
    readonly ServerDispatcher dispatcher;
    readonly ServerProcessor processor;

    public ManagerTCP(IContextHandler iContextHandler)
    {
        dispatcher = new(iContextHandler);
        processor = new(dispatcher);

        server.Start(processor.Process);
    }
}
}
