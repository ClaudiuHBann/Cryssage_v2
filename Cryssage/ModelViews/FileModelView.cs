using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class FileModelView : ObservableObject
{
    public FileModelView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<MessageFileModel> items;
}
}
