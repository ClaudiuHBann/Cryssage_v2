using Parser.Message;

using Networking.Context.Interface;

namespace Networking.Context
{
public class ContextProgress : IContext
{
    public enum Type_ : byte
    {
        SEND,
        RECEIVE
    }

    public uint Current { get; set; } = 0;
    public uint Total { get; set; } = 0;
    public float Percentage { get; set; } = 0f;
    public bool Done => MathF.Ceiling(Percentage) == 100;
    public Type_ TypeProgress { get; set; }

    public ContextProgress(Type_ type, uint total, Guid guid) : base(Message.Type.PROGRESS, guid)
    {
        Total = total;
        TypeProgress = type;
    }

    public float SetPercentage(uint current)
    {
        Current = current;
        Percentage = Current / Total * 100f;

        return Percentage;
    }
}
}
