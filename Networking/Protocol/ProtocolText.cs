using Parser;
using Parser.Message;

using Networking.Context;
using Networking.Interface;

namespace Networking.Protocol
{
public class ProtocolText : IProtocol
{
    public ProtocolText(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override Message Exchange(IContext context)
    {
        return MessageManager.ToMessageAck();
    }
}
}
