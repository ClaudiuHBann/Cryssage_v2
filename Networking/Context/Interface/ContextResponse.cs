using Parser.Message;

namespace Networking.Context.Interface
{
public class ContextResponse : IContext
{
    public Message.Type TypeRespond { get; set; }

    public ContextResponse(Message.Type type, Guid guid) : base(Message.Type.RESPONSE, guid)
    {
        TypeRespond = type;
    }
}
}
