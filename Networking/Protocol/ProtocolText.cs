using Networking.Protocol.Context.Operation;
using Parser.Message;

namespace Networking.Protocol
{
public class ProtocolText : IProtocol
{
    public ContextOperationText ContextOperation { get; set; }

    public ProtocolText(ContextOperationText contextOperation)
    {
        ContextOperation = contextOperation;
    }

    public Message Exchange(Message message)
    {
        throw new NotImplementedException();
    }
}
}
