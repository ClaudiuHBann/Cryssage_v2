using Parser.Message;

using Networking.Manager;

using Networking.Context;
using Networking.Context.Interface;

using Networking.Protocol;
using Networking.Protocol.File;
using Networking.Context.Response;

namespace Networking.TCP.Client
{
public class ClientDispatcher : IDispatcher
{
    public IContextHandler ContextHandler { get; set; }
    readonly ManagerFileTransfer managerFileTransfer;

    public ClientDispatcher(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    // dispatches sending requests and basic messages
    public override IContext Dispatch(IContext context)
    {
        if (context.Type == Message.Type.PROGRESS)
        {
            DispatchProgress((ContextProgress)context);

            // this context shouldn't be send
            return new ContextError();
        }

        IProtocol? protocol = null;
        switch (context.Type)
        {
        case Message.Type.REQUEST: {
            switch (((ContextRequest)context).TypeRequest)
            {
            case Message.Type.DISCOVER:
                protocol = new ProtocolDiscover(ContextHandler);
                break;
            case Message.Type.FILE:
                protocol = new ProtocolFileRequest(ContextHandler, managerFileTransfer);
                break;
            }
        }
        break;

        case Message.Type.TEXT:
            protocol = new ProtocolText(ContextHandler);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFileInfo(ContextHandler, managerFileTransfer);
            break;
        }

        return protocol != null ? protocol.GetNextContext(context) : new ContextError();
    }

    // dispatches responses to requests
    public IContext DispatchResponse(IContext context)
    {
        if (context.Type == Message.Type.PROGRESS)
        {
            DispatchProgress((ContextProgress)context);

            // this context shouldn't be send
            return new ContextError();
        }

        // not the first check because dispatchers need to handle progress contexts too
        // and in this manner we allow them to be dispather and anything else that comes
        // here and isn't a request we return an error
        if (context.Type != Message.Type.REQUEST)
        {
            return new ContextError();
        }

        IProtocol? protocol = null;
        switch (((ContextRequest)context).TypeRequest)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover(ContextHandler);
            break;
        case Message.Type.FILE:
            protocol = new ProtocolFileData(ContextHandler, managerFileTransfer);
            break;
        }

        return protocol != null ? protocol.GetNextContext(context) : new ContextError();
    }
}
}
