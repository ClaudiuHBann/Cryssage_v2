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

    public Type_ TypeProgress { get; set; }

    public float Current { get; set; } = 0;
    public float Total { get; set; } = 0;

    public byte Percentage { get; set; } = 0;
    public bool Done => MathF.Ceiling(Percentage) == 100;

    public ContextProgress(Type_ type, uint total, Guid guid) : base(Message.Type.PROGRESS, guid)
    {
        Total = total;
        TypeProgress = type;
    }

    public float SetPercentage(uint current)
    {
        Current = current;
        Percentage = (byte)(Current / Total * 100f);

        return Percentage;
    }

    // this method is used for sending data
    public override byte[] ToStream() =>
        throw new NotSupportedException($"A {nameof(ContextProgress)} should not be sent!");
}
}
