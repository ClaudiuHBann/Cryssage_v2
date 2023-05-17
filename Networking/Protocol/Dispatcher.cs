using Parser;
using Parser.Message;

using Networking.Context;
using Networking.Protocol.File;

namespace Networking.Protocol
{
public class Dispatcher
{
    public static Message Dispatch(Message message, IContext context)
    {
        IProtocol? protocol = null;
        switch (message.PacketMetadata.Header.Type)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover(context);
            break;
        case Message.Type.TEXT:
            protocol = new ProtocolText(context);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFileInfo(context);
            break;
        case Message.Type.FILE_REQUEST:
            protocol = new ProtocolFileRequest(context);
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFileData(context);
            break;
        }

        return protocol != null ? protocol.Exchange(message)
                                : MessageManager.ToMessage(Array.Empty<byte>(), Message.Type.ERROR);
    }
}
}
