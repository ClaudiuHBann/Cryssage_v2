using Parser;
using Parser.Message;

using Networking.Context;
using Networking.Interface;

namespace Networking.Protocol.File
{
public class ProtocolFileInfo : IProtocol
{
    public ProtocolFileInfo(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override Message Exchange(IContext context)
    {
        return MessageManager.ToMessageAck();
    }
}
}
