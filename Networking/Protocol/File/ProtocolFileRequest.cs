using Networking.Context;
using Networking.Manager;

namespace Networking.Protocol.File
{
public class ProtocolFileRequest : IProtocol
{
    readonly ManagerTransferFile ManagerTransferFile;

    public ProtocolFileRequest(IContextHandler contextHandler, ManagerTransferFile managerTransferFile)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        ManagerTransferFile = managerTransferFile;
    }

    public override IContext Exchange(IContext context)
    {
        return IContext.CreateACK();
    }
}
}
