using Networking.Context;

namespace Networking.Protocol.File
{
public class ProtocolFileData : IProtocol
{
    public ProtocolFileData(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override IContext Exchange(IContext context)
    {
        return IContext.CreateACK();
    }
}
}
