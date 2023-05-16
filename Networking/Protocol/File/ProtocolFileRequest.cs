using Parser.Message;

using Networking.Context.File;

namespace Networking.Protocol.File
{
public class ProtocolFileRequest : IProtocolFile
{
    public ProtocolFileRequest(IContextFile context) : base(context)
    {
    }

    public override Message ExchangeFile(Message message)
    {
        throw new NotImplementedException();
    }
}
}
