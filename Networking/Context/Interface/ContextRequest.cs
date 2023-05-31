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

    // this method is used for sending data
    public override byte[] ToStream() =>
        throw new NotSupportedException($"A {nameof(ContextProgress)} should not be sent!");
}
}
