using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class ContextOperationText : IContextOperation
{
    public Guid GUIDText { get; set; }

    public ContextOperationText(Guid guidChat, Guid guidText) : base(Message.Type.TEXT, guidChat)
    {
        GUIDText = guidText;
    }
}
}
