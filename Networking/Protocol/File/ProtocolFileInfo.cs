using Networking.Manager;

using Networking.Context;
using Networking.Context.Response;
using Networking.Context.Interface;

namespace Networking.Protocol.File
{
    public class ProtocolFileInfo : IProtocol
{
    readonly ManagerFileTransfer managerFileTransfer;

    public ProtocolFileInfo(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    public override IContext GetNextContext(IContext context)
    {
        if (!context.Responded)
        {
            context.Responded = true;

            managerFileTransfer.Add((ContextFileInfo)context);

            return context;
        }
        else
        {
            return new ContextEOS();
        }
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnReceiveFileInfo((ContextFileInfo)context);
        return new ContextACK();
    }
}
}
