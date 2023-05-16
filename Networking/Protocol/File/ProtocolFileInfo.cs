using Parser.Message;

using Networking.Context.File;

namespace Networking.Protocol.File
{
public class ProtocolFileInfo : IProtocolFile
{
    public ProtocolFileInfo(IContextFile context) : base(context)
    {
    }

    public override Message ExchangeFile(Message message)
    {
        throw new NotImplementedException();
    }
}
}
