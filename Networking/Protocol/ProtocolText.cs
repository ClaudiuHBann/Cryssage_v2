using Networking.Context;
using Networking.Context.Response;
using Networking.Context.Interface;

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
        ContextHandler.OnReceiveText((ContextText)context);
        return new ContextACK();
    }
}
}
