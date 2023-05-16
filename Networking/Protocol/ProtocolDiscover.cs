using Parser.Message;

using Networking.Context;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    public ProtocolDiscover(IContext context) : base(context)
    {
    }

    public override Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
