using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

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
    [property:JsonIgnore]
    float progress;

    // downloading started
    // used for download progressbar and label for IsVisible
    [ObservableProperty]
    [property:JsonIgnore]
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
