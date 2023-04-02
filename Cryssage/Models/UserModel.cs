using CommunityToolkit.Mvvm.ComponentModel;

using Cryssage.Views;

namespace Cryssage.Models
{
    public partial class UserModel : ObservableObject
    {
        public UserModel()
        {
            MessageView = new();
        }

        [ObservableProperty]
        string id;

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
    }
}
