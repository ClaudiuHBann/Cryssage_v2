using Networking.Context.Discover;
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
            return new ContextDiscover(Environment.MachineName);
        }
        else
        {
            return IContext.CreateEOS();
        }
    }

    public override IContext Exchange(IContext context)
    {
        ContextHandler.OnDiscover((ContextDiscover)context);
        return IContext.CreateACK();
    }
}
}
