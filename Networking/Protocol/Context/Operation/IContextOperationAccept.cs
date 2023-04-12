using Networking.TCP.Client;
using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
class IContextOperationAccept : IContextOperation
{
    public TCPClient Client { get; set; }

    public IContextOperationAccept(Guid guidChat, TCPClient client) : base(Message.Type.DISCOVER, guidChat)
    {
        Client = client;
    }
}
}
