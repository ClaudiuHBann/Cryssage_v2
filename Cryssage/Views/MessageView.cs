using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using Cryssage.Models;

namespace Cryssage.Views
{
public partial class MessageView : ObservableObject
{
    public MessageView()
    {
        Items = new();
    }

    [ObservableProperty]
    ObservableCollection<MessageModel> items;
}
}
