using Parser.Message;

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
        if (context.Type == Message.Type.REQUEST)
        {
            return new ContextDiscover(Environment.MachineName);
        }
        else
        {
            ContextHandler.OnDiscover((ContextDiscover)context);
            return IContext.CreateACK();
        }
    }
}
}
