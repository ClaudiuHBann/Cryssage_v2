using Networking.Manager;

using Networking.Context;
using Networking.Context.File;
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

            var contextFileInfo = (ContextFileInfo)context;
            managerFileTransfer.Add(
                new ContextFileRequest(contextFileInfo.Path, contextFileInfo.Size, contextFileInfo.GUID));

            return context;
        }
        else
        {
            return IContext.CreateEOS();
        }
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnReceiveFileInfo((ContextFileInfo)context);
        return IContext.CreateACK();
    }
}
}
