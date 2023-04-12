using Networking.Protocol.Context.Operation;
using Parser.Message;

namespace Networking.Protocol
{
public class ProtocolDispatcher
{
    public static Message? Dispatch(Message message, IContextOperation contextOperation)
    {
        IProtocol? protocol = null;
        switch (message.PacketMetadata.Header.Type)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover((ContextOperationAccept)contextOperation);
            break;
        case Message.Type.TEXT:
            protocol = new ProtocolText((ContextOperationText)contextOperation);
            break;
        case Message.Type.FILE:
            protocol = new ProtocolFile((ContextOperationFile)contextOperation);
            break;
        }

        return protocol?.Exchange(message);
    }
}
}
