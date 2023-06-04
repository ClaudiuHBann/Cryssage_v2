using Networking.Context;
using Networking.Context.Discover;

namespace Networking
{
public abstract class IContextHandler
{
    public string Name { get; set; } = Environment.MachineName;

    // discover
    public abstract void OnDiscover(ContextDiscover context);

    // send
    public abstract void OnSendProgress(ContextProgress context);

    // receive
    public abstract void OnReceiveText(ContextText context);
    public abstract void OnReceiveFileInfo(ContextFileInfo context);
    public abstract void OnReceiveProgress(ContextProgress context);
}
}
