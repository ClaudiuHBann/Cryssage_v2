using Parser.Message;

using Networking.Manager;

using Networking.Context;
using Networking.Context.Interface;

using Networking.Protocol;
using Networking.Protocol.File;

namespace Networking.TCP.Client
{
public class ClientDispatcher
{
    public IContextHandler ContextHandler { get; set; }
    readonly ManagerFileTransfer managerFileTransfer;

    public ClientDispatcher(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    IContext DispatchProgress(ContextProgress contextProgress)
    {
        // the client will send amounts of data and needs to invoke progress
        // the client receives just a ack or error so no progress for receiving
        if (contextProgress.TypeProgress == ContextProgress.Type_.SEND)
        {
            ContextHandler.OnSendProgress(contextProgress);
        }

        // this context shouldn't be send
        return IContext.CreateError();
    }

    // dispatches sending requests and basic messages
    public IContext Dispatch(IContext context)
    {
        if (context.Type == Message.Type.PROGRESS)
        {
            return DispatchProgress((ContextProgress)context);
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

        return protocol != null ? protocol.GetNextContext(context) : IContext.CreateError();
    }

    // dispatches responses to requests
    public IContext DispatchResponse(IContext context)
    {
        if (context.Type == Message.Type.PROGRESS)
        {
            return DispatchProgress((ContextProgress)context);
        }

        // not the first check because dispatchers need to handle progress contexts too
        // and in this manner we allow them to be dispather and anything else that comes
        // here and isn't a request we return an error
        if (context.Type != Message.Type.REQUEST)
        {
            return IContext.CreateError();
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

        return protocol != null ? protocol.GetNextContext(context) : IContext.CreateError();
    }
}
}
