using Networking.Manager;
using Networking.Context;
using Networking.Context.File;

namespace Networking.Protocol.File
{
public class ProtocolFileInfo : IProtocol
{
    readonly ManagerTransferFile ManagerTransferFile;

    public ProtocolFileInfo(IContextHandler contextHandler, ManagerTransferFile managerTransferFile)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        ManagerTransferFile = managerTransferFile;
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnReceiveFileInfo((ContextFileInfo)context);
        return IContext.CreateACK();
    }
}
}
