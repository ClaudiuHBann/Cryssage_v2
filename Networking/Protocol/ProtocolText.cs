using Networking.Context;
using Parser.Message;

namespace Networking.Protocol
{
    public class ProtocolText : IProtocol
{
    public ContextText ContextOperation { get; set; }

    public ProtocolText(ContextText contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
