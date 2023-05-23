using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class UserView : ObservableObject
{
    public UserView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<UserModel> items;
}
}
