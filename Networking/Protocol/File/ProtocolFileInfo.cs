using Networking.Context;

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
        return IContext.CreateACK();
    }
}
}
