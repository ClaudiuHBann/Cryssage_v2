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
    DateTime time;

    [ObservableProperty]
    string message;

    [ObservableProperty]
    MessageView messageView;

    public UserModel()
    {
        MessageView = new();
    }

    public UserModel(string ip, string avatar, string name, DateTime time, string message)
    {
        Ip = ip;
        Avatar = avatar;
        Name = name;
        Time = time;
        Message = message;

        MessageView = new();
    }
}
}
