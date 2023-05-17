namespace Networking.Context
{
public class IProgress
{
    public uint Current { get; set; } = 0;
    public uint Total { get; set; } = 0;
    public float Percentage { get; set; } = 0f;

    public float SetPercentage(uint current)
    {
        Current = current;
        Percentage = Current / Total * 100f;

        return Percentage;
    }

    public bool IsContextProgress() => Current != 0 || Total != 0 || Percentage != 0f;
}
}
