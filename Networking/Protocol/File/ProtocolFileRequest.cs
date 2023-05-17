using Parser.Message;

using Networking.Context;

namespace Networking.Protocol.File
{
public class ProtocolFileRequest : IProtocol
{
    public ProtocolFileRequest(IContext context) : base(context)
    {
    }

    public override Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
