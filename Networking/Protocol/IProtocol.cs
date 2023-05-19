using Parser.Message;

using Networking.Context;
using Networking.Interface;

namespace Networking.Protocol
{
public abstract class IProtocol
{
    public IContextHandler ContextHandler { get; set; }

    public IProtocol(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public abstract Message Exchange(IContext context);
}
}
