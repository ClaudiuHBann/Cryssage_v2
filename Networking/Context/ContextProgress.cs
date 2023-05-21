using Parser.Message;

namespace Networking.Context
{
public class ContextProgress : IContext
{
    public uint Current { get; set; } = 0;
    public uint Total { get; set; } = 0;
    public float Percentage { get; set; } = 0f;
    public bool Done => MathF.Ceiling(Percentage) == 100;

    public ContextProgress(uint total, Guid guid) : base(Message.Type.PROGRESS, guid)
    {
        Total = total;
    }

    public float SetPercentage(uint current)
    {
        Current = current;
        Percentage = Current / Total * 100f;

        return Percentage;
    }
}
}
