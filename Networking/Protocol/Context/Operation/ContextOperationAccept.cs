using Networking.TCP.Client;
using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class ContextOperationAccept : IContextOperation
{
    public TCPClient Client { get; set; }

    public ContextOperationAccept(Guid guidChat, TCPClient client) : base(Message.Type.DISCOVER, guidChat)
    {
        Client = client;
    }
}
}
