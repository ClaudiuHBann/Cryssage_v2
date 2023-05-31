using Parser.Message;

using Networking.Manager;

using Networking.Context;
using Networking.Context.Response;
using Networking.Context.Interface;

using Networking.Protocol;
using Networking.Protocol.File;

namespace Networking.TCP.Server
{
public class ServerDispatcher : IDispatcher
{
    public IContextHandler ContextHandler { get; set; }
    readonly ManagerFileTransfer managerFileTransfer;
    readonly ManagerConnection managerConnection;

    public ServerDispatcher(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer,
                            ManagerConnection managerConnection)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
        this.managerConnection = managerConnection;
    }

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

        var contextResponse = protocol != null ? protocol.Exchange(context) : new ContextError();
        if (context.Type == Message.Type.REQUEST)
        {
            managerConnection.Respond(context.IP, (ContextRequest)context);
        }
        return contextResponse;
    }
}
}
