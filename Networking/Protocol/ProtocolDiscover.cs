using Networking.Context.Discover;
using Networking.Context.Response;
using Networking.Context.Interface;

namespace Networking.Protocol
{
    public class ProtocolDiscover : IProtocol
{
    public ProtocolDiscover(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext GetNextContext(IContext context)
    {
        if (!context.Responded)
        {
            context.Responded = true;
            return new ContextDiscover(ContextHandler.Name);
        }
        else
        {
            return new ContextEOS();
        }
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnDiscover((ContextDiscover)context);
        return new ContextACK();
    }
}
}
