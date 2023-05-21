using Parser.Message;

using Networking.Context;

namespace Networking.Protocol
{
public abstract class IProtocol
{
    public IContextHandler ContextHandler { get; set; }

    public IProtocol(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public abstract IContext Exchange(IContext context);
}
}
