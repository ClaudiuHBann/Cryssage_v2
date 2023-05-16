using Networking.Context;
using Parser.Message;

namespace Networking.Protocol
{
    public class ProtocolDiscover : IProtocol
{
    public ContextAccept ContextOperation { get; set; }

    public ProtocolDiscover(ContextAccept contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
