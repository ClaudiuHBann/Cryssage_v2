using Parser;
using Parser.Message;

using Networking.Context;

using Networking.Protocol.File;

namespace Networking.Protocol
{
public class Dispatcher
{
    public IContextHandler ContextHandler { get; set; }

    public Dispatcher(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public Message Dispatch(IContext context)
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
            protocol = new ProtocolFileInfo(ContextHandler);
            break;
        case Message.Type.FILE_REQUEST:
            protocol = new ProtocolFileRequest(ContextHandler);
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFileData(ContextHandler);
            break;
        }

        return protocol != null ? protocol.Exchange(context) : MessageManager.ToMessageError();
    }
}
}
