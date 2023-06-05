using CommunityToolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

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
    [property:JsonIgnore]
    bool online;

    [ObservableProperty]
    MessageModelView messageView;

    public void FireOnPropertyChangedMessageView() => OnPropertyChanged(nameof(MessageView));

    [ObservableProperty]
    FileModelView fileView;

    public static readonly Color ColorStroke = new(255, 255, 255);
    public static readonly Color ColorFill = new(5, 174, 19);
    public static readonly Color ColorTransparent = new(0, 0, 0, 0);

    public UserModel(string ip, string avatar, string name)
    {
        Ip = ip;
        Avatar = avatar;
        Name = name;
        Online = false;

        MessageView = new();
        FileView = new();
    }
}
}
