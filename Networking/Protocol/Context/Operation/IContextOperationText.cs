using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class IContextOperationText : IContextOperation
{
    public Guid GUIDText { get; set; }

    public IContextOperationText(Guid guidChat, Guid guidText) : base(Message.Type.TEXT, guidChat)
    {
        GUIDText = guidText;
    }
}
}
