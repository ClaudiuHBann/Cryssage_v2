using Networking.Manager;

using Networking.Context.File;
using Networking.Context.Response;
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
        if (!context.Responded)
        {
            context.Responded = true;

            managerFileTransfer.Add((ContextFileRequest)context);

            return context;
        }
        else
        {
            return new ContextEOS();
        }
    }

    public override IContext Exchange(IContext context)
    {
        managerFileTransfer.Add((ContextFileRequest)context);
        return new ContextACK();
    }
}
}
