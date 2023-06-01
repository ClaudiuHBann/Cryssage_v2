using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class MessageModelView : ObservableObject
{
    public MessageModelView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<MessageModel> items;
}
}
