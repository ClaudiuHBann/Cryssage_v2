using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class UserModelView : ObservableObject
{
    public UserModelView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<UserModel> items;
}
}
