using Parser.Message;

using Networking.Context;

namespace Networking.Protocol.File
{
public class ProtocolFileInfo : IProtocol
{
    public ProtocolFileInfo(IContext context) : base(context)
    {
    }

    public override Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
