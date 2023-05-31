using Parser.Message;

using Networking.Manager;

using Networking.Context;
using Networking.Context.Interface;

using Networking.Protocol;
using Networking.Protocol.File;

namespace Networking.TCP.Server
{
public class ServerDispatcher
{
    public IContextHandler ContextHandler { get; set; }
    readonly ManagerFileTransfer managerFileTransfer;
    readonly ManagerConnection managerConnection;

    public ServerDispatcher(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer,
                            ManagerConnection managerConnection)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
        this.managerConnection = managerConnection;
    }

    IContext DispatchProgress(ContextProgress contextProgress)
    {
        // the server will receive amounts of data and needs to invoke progress
        // the server sends just a error or eos so no progress for sending
        if (contextProgress.TypeProgress == ContextProgress.Type_.RECEIVE)
        {
            ContextHandler.OnReceiveProgress(contextProgress);
        }

        // this context shouldn't be send
        return IContext.CreateError();
    }

    public IContext Dispatch(IContext context)
    {
        if (context.Type == Message.Type.PROGRESS)
        {
            return DispatchProgress((ContextProgress)context);
        }

        IProtocol? protocol = null;
        switch (context.Type)
        {
        case Message.Type.REQUEST:
            switch (((ContextRequest)context).TypeRequest)
            {
            case Message.Type.DISCOVER:
                // we don't need to process the discover message
                break;
            case Message.Type.FILE:
                protocol = new ProtocolFileRequest(ContextHandler, managerFileTransfer);
                break;
            }
            break;

        case Message.Type.RESPONSE:
            switch (((ContextResponse)context).TypeResponse)
            {
            case Message.Type.DISCOVER:
                protocol = new ProtocolDiscover(ContextHandler);
                break;
            }
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFileData(ContextHandler, managerFileTransfer);
            break;

        case Message.Type.TEXT:
            protocol = new ProtocolText(ContextHandler);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFileInfo(ContextHandler, managerFileTransfer);
            break;
        }

        var contextResponse = protocol != null ? protocol.Exchange(context) : IContext.CreateError();
        if (context.Type == Message.Type.REQUEST)
        {
            managerConnection.Respond(context.IP, (ContextRequest)context);
        }
        return contextResponse;
    }
}
}
