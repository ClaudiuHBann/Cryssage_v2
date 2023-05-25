using Networking.Context;
using Networking.Context.File;

namespace Networking.Protocol.File
{
public class ProtocolFileInfo : IProtocol
{
    public ProtocolFileInfo(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnReceiveFileInfo((ContextFileInfo)context);
        return IContext.CreateACK();
    }
}
}
