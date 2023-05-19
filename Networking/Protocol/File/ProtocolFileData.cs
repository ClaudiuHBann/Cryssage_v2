using Parser;
using Parser.Message;

using Networking.Context;
using Networking.Interfaces;

namespace Networking.Protocol.File
{
public class ProtocolFileData : IProtocol
{
    public ProtocolFileData(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override Message Exchange(IContext context)
    {
        return MessageManager.ToMessageAck();
    }
}
}
