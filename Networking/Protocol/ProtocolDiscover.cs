using Networking.Context;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    public ProtocolDiscover(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext Exchange(IContext context)
    {
        return IContext.CreateACK();
    }
}
}
