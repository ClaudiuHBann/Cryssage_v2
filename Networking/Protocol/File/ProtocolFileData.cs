using Parser.Message;

using Networking.Context;

namespace Networking.Protocol.File
{
public class ProtocolFileData : IProtocol
{
    public ProtocolFileData(IContext context) : base(context)
    {
    }

    public override Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
