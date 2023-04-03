using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public partial class MessageTextModel : MessageModel
{
    [ObservableProperty]
    string text;

    public MessageTextModel(string id, MessageType type, string sender, DateTime timestamp, MessageState state,
                            bool mine, string text)
        : base(id, type, sender, timestamp, state, mine)
    {
        Text = text;
    }
}
}
