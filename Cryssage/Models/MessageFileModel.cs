using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
public partial class MessageFileModel : MessageModel
{
    [ObservableProperty]
    ImageSource icon;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    int size;
}
}
