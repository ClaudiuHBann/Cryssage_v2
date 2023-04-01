using System.Net.Sockets;

using Parser;
using Parser.Message;
using Parser.Message.Header;

namespace Networking.TCP.Client
{
using CallbackSend = Action<SocketError, uint>;
using CallbackReceive = Action<SocketError, MessageDisassembled?>;

public class TCPClient : TCPClientRaw
{
    public TCPClient() : base()
    {
    }

    public TCPClient(Socket socket) : base(socket)
    {
    }

    public void Send(byte[] stream, Message.Type type, CallbackSend? callback = null)
    {
        var message = MessageManager.ToMessage(stream, type);
        var messageBytes = MessageConverter.MessageToBytes(message);

        SendAll(messageBytes, callback);
    }

    public void Receive(CallbackReceive callback)
    {
        byte[] metadataBytes = new byte[HeaderMetadata.SIZE];
        ReceiveAll(metadataBytes, (error, bytesMetadata) =>
                                  {
                                      if (error != SocketError.Success || bytesMetadata == null)
                                      {
                                          callback(error, null);
                                          return;
                                      }

                                      var packetMetadata = MessageConverter.BytesToPacketMetadata(bytesMetadata);
                                      var data = new byte[packetMetadata.Header.Size];
                                      ReceiveAll(data,
                                                 (error, bytesData) =>
                                                 {
                                                     if (error != SocketError.Success || bytesData == null)
                                                     {
                                                         callback(error, null);
                                                         return;
                                                     }

                                                     var bytes = new byte[bytesMetadata.Length + bytesData.Length];
                                                     bytesMetadata.CopyTo(bytes, 0);
                                                     bytesData.CopyTo(bytes, bytesMetadata.Length);

                                                     var message = MessageConverter.BytesToMessage(bytes);
                                                     var messageDisassembled = MessageManager.FromMessage(message);

                                                     callback(error, messageDisassembled);
                                                 });
                                  });
    }
}
}
