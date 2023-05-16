using Parser.Message;

using Networking.Context;

namespace Networking.Protocol
{
public abstract class IProtocol
{
    public IContext Context { get; set; }

    public IProtocol(IContext context)
    {
        Context = context;
    }

    public abstract Message Exchange(Message message);
}
}
