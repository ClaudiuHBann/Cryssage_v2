using Networking.Context.File;
using Parser.Message;

namespace Networking.Protocol
{
    public class ProtocolFile : IProtocol
{
    public ContextFileRequest ContextOperation { get; set; }

    public ProtocolFile(ContextFileRequest contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
