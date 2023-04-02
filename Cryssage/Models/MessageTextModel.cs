using CommunityToolkit.Mvvm.ComponentModel;

namespace Cryssage.Models
{
    public partial class MessageTextModel : MessageModel
    {
        [ObservableProperty]
        string text;
    }
}
