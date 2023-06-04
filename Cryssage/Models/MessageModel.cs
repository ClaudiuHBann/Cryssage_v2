using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public enum MessageType : byte
{
    TEXT,
    FILE
}

public partial class MessageModel : ObservableObject
{
    [ObservableProperty]
    Guid guid;

    [ObservableProperty]
    MessageType type;

    [ObservableProperty]
    string sender;

    [ObservableProperty]
    DateTime timestamp;

    [ObservableProperty]
    bool mine;

    public static readonly Color LeftColor = new(32, 44, 51);
    public static readonly Color RightColor = new(8, 92, 76);

    public static readonly LayoutOptions LeftLayoutOption = LayoutOptions.Start;
    public static readonly LayoutOptions RightLayoutOption = LayoutOptions.End;

    private const int LeftMarginTop = 10;
    private const int LeftMarginBottom = 10;
    public static readonly Thickness LeftMargin = new(15, LeftMarginTop, 15, LeftMarginBottom);
    public static readonly Thickness RightMargin = new(15, LeftMarginTop, 15, LeftMarginBottom);

    public MessageModel(Guid guid, MessageType type, string sender, DateTime timestamp, bool mine)
    {
        Guid = guid;
        Type = type;
        Sender = sender;
        Timestamp = timestamp;
        Mine = mine;
    }
}
}
