using System.Net.Sockets;

using Parser.Message;

namespace Cryssage.Events
{
class EventsNetworking
{
    public delegate void DelegateOnMessageReceiveError(SocketError error);
    public delegate void DelegateOnMessageReceive(MessageDisassembled messageDisassembled);
    public delegate void DelegateOnMessageSendError(SocketError error);
    public delegate void DelegateOnMessageSend(uint bytesTransferred);

    public DelegateOnMessageReceiveError OnMessageReceiveError { get; set; } = new((_) =>
                                                                                       {});
    public DelegateOnMessageReceive OnMessageReceive { get; set; } = new((_) =>
                                                                             {});
    public DelegateOnMessageSendError OnMessageSendError { get; set; } = new((_) =>
                                                                                 {});
    public DelegateOnMessageSend OnMessageSend { get; set; } = new((_) =>
                                                                       {});
}
}
