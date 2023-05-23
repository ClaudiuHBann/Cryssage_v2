using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public partial class MessageTextModel : MessageModel
{
    [ObservableProperty]
    string text;

    public MessageTextModel(string sender, DateTime timestamp, MessageState state, bool mine, string text)
        : base(MessageType.TEXT, sender, timestamp, state, mine)
    {
        Text = text;
    }
}
}
