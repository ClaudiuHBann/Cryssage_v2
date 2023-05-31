using Networking.Manager;

using Networking.Context.File;
using Networking.Context.Interface;

namespace Networking.Protocol.File
{
    public class ProtocolFileData : IProtocol
{
    readonly ManagerFileTransfer managerFileTransfer;

    public ProtocolFileData(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    public override IContext GetNextContext(IContext context)
    {
        var stream = managerFileTransfer.Read(context.GUID);
        return stream != null ? new ContextFileData(stream, context.GUID) : IContext.CreateEOS();
    }

    public override IContext Exchange(IContext context)
    {
        var contextFileData = (ContextFileData)context;
        return managerFileTransfer.Write(contextFileData.GUID, contextFileData.Stream) ? IContext.CreateACK()
                                                                                       : IContext.CreateError();
    }
}
}
