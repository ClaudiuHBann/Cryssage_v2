using Cryssage.Models;

namespace Cryssage.Events
{
class EventsUI
{
    public delegate void DelegateOnMessageAdd(MessageModel message);

    public DelegateOnMessageAdd OnMessageAdd { get; set; } = new((_) =>
                                                                     {});
}
}
