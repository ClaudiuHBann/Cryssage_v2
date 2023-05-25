using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class FileView : ObservableObject
{
    public FileView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<MessageFileModel> items;
}
}
