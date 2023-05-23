using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public partial class MessageFileModel : MessageModel
{
    [ObservableProperty]
    string icon;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    uint size;

    public MessageFileModel(string sender, DateTime timestamp, MessageState state, bool mine, string icon, string name,
                            uint size, Guid? guid = null)
        : base(guid ?? Guid.NewGuid(), MessageType.FILE, sender, timestamp, state, mine)
    {
        Icon = icon;
        Name = name;
        Size = size;
    }
}
}
