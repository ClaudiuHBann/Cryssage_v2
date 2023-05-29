using Parser.Message;
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
        // this is only for requests
        if (context.Type != Message.Type.REQUEST)
        {
            return IContext.CreateError();
        }

        // if we didn't responded we respond and keep that in mind
        // else we send the end of stream context
        var contextRequest = (ContextRequest)context;
        if (!contextRequest.Responded)
        {
            contextRequest.Responded = true;
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
