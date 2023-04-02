using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        [ObservableProperty]
        UserModel user;

        [RelayCommand]
        void Add()
        {
            Items.Add(User);
        }
    }
}
