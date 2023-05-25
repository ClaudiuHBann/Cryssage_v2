using Networking.Context;

namespace Networking.Protocol.File
{
public class ProtocolFileRequest : IProtocol
{
    public ProtocolFileRequest(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext Exchange(IContext context)
    {
        return IContext.CreateACK();
    }
}
}
