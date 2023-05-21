using Parser.Message;

using Networking.Context;
using Networking.Manager;

using Networking.Protocol.File;
using Networking.Protocol;

namespace Networking.TCP.Server
{
public class ServerDispatcher
{
    public IContextHandler ContextHandler { get; set; }
    readonly ManagerTransferFile ManagerTransferFile;

    public ServerDispatcher(IContextHandler contextHandler, ManagerTransferFile managerTransferFile)
    {
        ContextHandler = contextHandler;
        ManagerTransferFile = managerTransferFile;
    }

    public IContext Dispatch(IContext context)
    {
        IProtocol? protocol = null;
        switch (context.Type)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover(ContextHandler);
            break;
        case Message.Type.TEXT:
            protocol = new ProtocolText(ContextHandler);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFileInfo(ContextHandler, ManagerTransferFile);
            break;
        case Message.Type.FILE_REQUEST:
            protocol = new ProtocolFileRequest(ContextHandler, ManagerTransferFile);
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFileData(ContextHandler, ManagerTransferFile);
            break;
        }

        return protocol != null ? protocol.Exchange(context) : IContext.CreateError();
    }
}
}
