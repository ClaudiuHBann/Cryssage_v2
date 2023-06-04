using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public partial class MessageFileModel : MessageModel
{
    [ObservableProperty]
    string icon;

    [ObservableProperty]
    string filePath;

    [ObservableProperty]
    uint size;

    [ObservableProperty]
    float progress;

    // downloading started
    // used for download progressbar and label for IsVisible
    [ObservableProperty]
    bool progressStart;

    public MessageFileModel(string sender, DateTime timestamp, bool mine, string icon, string filePath, uint size,
                            Guid? guid = null)
        : base(guid ?? Guid.NewGuid(), MessageType.FILE, sender, timestamp, mine)
    {
        Icon = icon;
        FilePath = filePath;
        Size = size;

        Progress = 0;
        ProgressStart = false;
    }
}
}
