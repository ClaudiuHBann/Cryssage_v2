using Networking.Manager;
using Networking.Context.File;
using Networking.Context.Interface;

namespace Networking.Protocol.File
{
    public class ProtocolFileRequest : IProtocol
{
    readonly ManagerFileTransfer managerFileTransfer;

    public ProtocolFileRequest(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    public override IContext GetNextContext(IContext context)
    {
        managerFileTransfer.Add((ContextFileRequest)context);
        return base.GetNextContext(context);
    }

    public override IContext Exchange(IContext context)
    {
        managerFileTransfer.Add((ContextFileRequest)context);
        return IContext.CreateACK();
    }
}
}
