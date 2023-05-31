using Networking.Context.Response;
using Networking.Context.Interface;

namespace Networking.Protocol
{
    public abstract class IProtocol
{
    public IContextHandler ContextHandler { get; set; }

    public IProtocol(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    // for client
    // some of them are provided a request to respond to them
    // some of them are provided the context to be send
    public virtual IContext GetNextContext(IContext context)
    {
        if (!context.Responded)
        {
            context.Responded = true;
            return context;
        }
        else
        {
            return new ContextEOS();
        }
    }

    // for server
    public abstract IContext Exchange(IContext context);
}
}
