using Parser.Message;

using Networking.Context;
using Networking.Context.File;

namespace Networking.Protocol
{
public class Dispatcher
{
    public static Message? Dispatch(Message message, IContext contextOperation)
    {
        IProtocol? protocol = null;
        switch (message.PacketMetadata.Header.Type)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover((ContextAccept)contextOperation);
            break;
        case Message.Type.TEXT:
            protocol = new ProtocolText((ContextText)contextOperation);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFile((ContextFileRequest)contextOperation);
            break;
        case Message.Type.FILE_REQUEST:
            protocol = new ProtocolFile((ContextFileRequest)contextOperation);
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFile((ContextFileRequest)contextOperation);
            break;
        }

        return protocol?.Exchange(message);
    }
}
}
