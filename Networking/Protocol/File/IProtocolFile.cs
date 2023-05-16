using Parser.Message;

using Networking.Protocol.File;
using Networking.Context.File;

namespace Networking.Protocol
{
public abstract class IProtocolFile : IProtocol
{
    public IProtocolFile(IContextFile contextFile) : base(contextFile)
    {
    }

    public sealed override Message Exchange(Message message)
    {
        switch (Context.Type)
        {
        case Message.Type.FILE_INFO:
            return ((ProtocolFileInfo)this).Exchange(message);
        case Message.Type.FILE_REQUEST:
            return ((ProtocolFileRequest)this).Exchange(message);
        case Message.Type.FILE_DATA:
            return ((ProtocolFileData)this).Exchange(message);
        }

        throw new ArgumentException("IContext");
    }

    public abstract Message ExchangeFile(Message message);
}
}
