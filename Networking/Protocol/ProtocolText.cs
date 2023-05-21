using Networking.Context;

namespace Networking.Protocol
{
public class ProtocolText : IProtocol
{
    public ProtocolText(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext Exchange(IContext context)
    {
        return IContext.CreateACK();
    }
}
}
