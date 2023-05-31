using Networking.Context;
using Networking.Context.Interface;

namespace Networking.TCP
{
public abstract class IDispatcher
{
    readonly IContextHandler contextHandler;

    protected IDispatcher(IContextHandler contextHandler)
    {
        this.contextHandler = contextHandler;
    }

    public abstract IContext Dispatch(IContext context);

    protected virtual void DispatchProgress(ContextProgress contextProgress)
    {
        switch (contextProgress.TypeProgress)
        {
        case ContextProgress.Type_.SEND:
            // the server will receive amounts of data and needs to invoke progress
            // the server sends just a error or eos so no progress for sending
            contextHandler.OnSendProgress(contextProgress);
            break;
        case ContextProgress.Type_.RECEIVE:
            // the client will send amounts of data and needs to invoke progress
            // the client receives just a ack or error so no progress for receiving
            contextHandler.OnReceiveProgress(contextProgress);
            break;
        }
    }
}
}
