using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public enum MessageType
{
    TEXT,
    FILE
}

public enum MessageState
{
    LOADING,
    SENT,
    RECEIVED,
    SEEN
}

public partial class MessageModel : ObservableObject
{
    [ObservableProperty]
    string id;

    [ObservableProperty]
    MessageType type;

    [ObservableProperty]
    string sender;

    [ObservableProperty]
    DateTime timestamp;

    [ObservableProperty]
    MessageState state;

    [ObservableProperty]
    bool mine;

    public static readonly Color LeftColor = new(32, 44, 51);
    public static readonly Color RightColor = new(8, 92, 76);

    public static readonly LayoutOptions LeftLayoutOption = LayoutOptions.Start;
    public static readonly LayoutOptions RightLayoutOption = LayoutOptions.End;

    private const int LeftMarginBottom = 5;
    public static readonly Thickness LeftMargin = new(10, 0, 0, LeftMarginBottom);
    public static readonly Thickness RightMargin = new(0, 0, 10, LeftMarginBottom);

    public MessageModel()
    {
    }

    public MessageModel(string id, MessageType type, string sender, DateTime timestamp, MessageState state, bool mine)
    {
        Id = id;
        Type = type;
        Sender = sender;
        Timestamp = timestamp;
        State = state;
        Mine = mine;
    }
}
}
