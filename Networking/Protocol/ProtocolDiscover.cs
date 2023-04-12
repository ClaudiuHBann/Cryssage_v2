using Networking.Protocol.Context.Operation;
using Parser.Message;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    public ContextOperationAccept ContextOperation { get; set; }

    public ProtocolDiscover(ContextOperationAccept contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
