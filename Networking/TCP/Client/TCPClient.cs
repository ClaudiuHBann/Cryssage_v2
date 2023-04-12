using System.Net.Sockets;
using Networking.Protocol.Context.Operation;
using Parser;
using Parser.Message;
using Parser.Message.Header;

namespace Networking.TCP.Client
{
using CallbackSend = Action<SocketError, uint>;
using CallbackSendShard = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, MessageDisassembled?>;
using CallbackReceiveShard = Action<SocketError, uint>;

public class TCPClient : TCPClientRaw
{
    public TCPClient() : base()
    {
    }

    public TCPClient(Socket socket) : base(socket)
    {
    }

    public void Send(byte[] stream, Message.Type type, CallbackSend? callbackSend = null,
                     CallbackSendShard? callbackSendShard = null, IContextOperation? contextOperation = null)
    {
        var message = MessageManager.ToMessage(stream, type);
        var messageBytes = MessageConverter.MessageToBytes(message);

        SendAll(messageBytes, callbackSend, callbackSendShard, contextOperation);
    }

    public void Receive(CallbackReceive callbackReceive, CallbackReceiveShard? callbackReceiveShard = null,
                        IContextOperation? contextOperation = null)
    {
        byte[] metadataBytes = new byte[HeaderMetadata.SIZE];
        ReceiveAll(metadataBytes,
                   (error, bytesMetadata) =>
                   {
                       if (error != SocketError.Success || bytesMetadata == null)
                       {
                           callbackReceive(error, null);
                           return;
                       }

                       var packetMetadata = MessageConverter.BytesToPacketMetadata(bytesMetadata);
                       var data = new byte[packetMetadata.Header.Size];
                       ReceiveAll(data,
                                  (error, bytesData) =>
                                  {
                                      if (error != SocketError.Success || bytesData == null)
                                      {
                                          callbackReceive(error, null);
                                          return;
                                      }

                                      var bytes = new byte[bytesMetadata.Length + bytesData.Length];
                                      bytesMetadata.CopyTo(bytes, 0);
                                      bytesData.CopyTo(bytes, bytesMetadata.Length);

                                      var message = MessageConverter.BytesToMessage(bytes);
                                      var messageDisassembled = MessageManager.FromMessage(message);

                                      callbackReceive(error, messageDisassembled);
                                  },
                                  callbackReceiveShard, contextOperation);
                   },
                   callbackReceiveShard, contextOperation);
    }
}
}
