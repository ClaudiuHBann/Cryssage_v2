using CommunityToolkit.Mvvm.ComponentModel;

using Cryssage.Views;

namespace Cryssage.Models
{
public partial class UserModel : ObservableObject
{
    [ObservableProperty]
    string ip;

    [ObservableProperty]
    string avatar;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    MessageModelView messageView;

    public void FireOnPropertyChangedMessageView() => OnPropertyChanged(nameof(MessageView));

    [ObservableProperty]
    FileModelView fileView;

    public UserModel(string ip, string avatar, string name)
    {
        Ip = ip;
        Avatar = avatar;
        Name = name;

        MessageView = new();
        FileView = new();
    }
}
}
