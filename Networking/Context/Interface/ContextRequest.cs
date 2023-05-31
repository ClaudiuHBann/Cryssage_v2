using Parser.Message;

namespace Networking.Context.Interface
{
public class ContextRequest : IContext
{
    public Message.Type TypeRequest { get; set; }

    public ContextRequest(Message.Type type, Guid guid) : base(Message.Type.REQUEST, guid)
    {
        TypeRequest = type;
    }
}
}
