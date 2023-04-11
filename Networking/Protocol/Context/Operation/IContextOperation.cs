using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class IContextOperation
{
    public enum OperationState : byte
    {
        UNKNOWN,
        BEGIN,
        PROGRESS,
        END
    }

    public Guid GUIDOperation { get; set; } = Guid.NewGuid();
    public Guid GUIDChat { get; set; }
    public Message.Type Type { get; set; } = Message.Type.UNKNOWN;
    public OperationState State { get; set; } = OperationState.UNKNOWN;
    public float Percentage { get; set; } = 0f;

    public IContextOperation(Message.Type type, Guid guidChat)
    {
        Type = type;
        GUIDChat = guidChat;
    }
}
}
