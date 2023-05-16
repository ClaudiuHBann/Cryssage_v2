using Parser.Message;

using Networking.Context;

namespace Networking.Protocol
{
public class ProtocolText : IProtocol
{
    public ProtocolText(IContext context) : base(context)
    {
    }

    public override Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
