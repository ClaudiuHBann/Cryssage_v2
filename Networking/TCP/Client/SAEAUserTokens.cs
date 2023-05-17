using System.Net.Sockets;

namespace Networking.TCP.Client
{
using CallbackSend = Action<SocketError, uint>;
using CallbackSendShard = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, byte[]?>;
using CallbackReceiveShard = Action<SocketError, uint>;

public class SAEAUserTokens
{
    // SocketAsyncEventArgs context for keeping sending callbacks
    class SAEAUserTokenSend
    {
        // invoked when operation finished
        public CallbackSend? CallbackSend { get; set; } = null;
        // invoked when operation is in progress
        public CallbackSendShard? CallbackSendShard { get; set; } = null;

        public SAEAUserTokenSend(CallbackSend? callbackSend = null, CallbackSendShard? callbackSendShard = null)
        {
            CallbackSend = callbackSend;
            CallbackSendShard = callbackSendShard;
        }
    }

    // SocketAsyncEventArgs context for keeping receiving callbacks
    class SAEAUserTokenReceive
    {
        // invoked when operation finished
        public CallbackReceive? CallbackReceive { get; set; } = null;
        // invoked when operation is in progress
        public CallbackReceiveShard? CallbackReceiveShard { get; set; } = null;

        public SAEAUserTokenReceive(CallbackReceive? callbackReceive = null,
                                    CallbackReceiveShard? callbackReceiveShard = null)
        {
            CallbackReceive = callbackReceive;
            CallbackReceiveShard = callbackReceiveShard;
        }
    }
}
}
