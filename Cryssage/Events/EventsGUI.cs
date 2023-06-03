using Networking.Context;

namespace Cryssage.Events
{
public class EventsGUI
{
    public delegate void OnProgressReceiveDelegate(ContextProgress contextProgress);
    public OnProgressReceiveDelegate OnProgressReceive { get; set; } = new(
        _ =>
            {});

    public delegate void OnProgressSendDelegate(ContextProgress contextProgress);
    public OnProgressSendDelegate OnProgressSend { get; set; } = new(
        _ =>
            {});
}
}
