using Parser.Message;
using Parser.Message.Header;
using Parser.Message.Packet;

namespace Parser
{
public class MessageManager
{
    public static Message.Message ToMessage(byte[] bytes, Message.Message.Type type)
    {
        return ToMessage(bytes, type, Guid.NewGuid());
    }

    public static Message.Message ToMessage(byte[] bytes, Message.Message.Type type, Guid guid)
    {
        List<PacketData> packetDatas = new();
        uint packetDatasSize = 0;

        var packetsCount = bytes.Length / PacketData.CONTENT_SIZE_MAX;
        for (long i = 0; i < packetsCount; i++)
        {
            HeaderData headerData = new(guid, (uint)i);
            var contentAsBytesRange =
                (int)(i * PacketData.CONTENT_SIZE_MAX)..(int)((i + 1) * PacketData.CONTENT_SIZE_MAX);

            PacketData packetData = new(headerData, bytes[contentAsBytesRange]);

            packetDatas.Add(packetData);
            packetDatasSize += packetData.Size;
        }

        var packetLastSize = bytes.Length % PacketData.CONTENT_SIZE_MAX;
        if (packetLastSize > 0)
        {
            HeaderData headerData = new(guid, (uint)packetsCount);
            PacketData packetData = new(headerData, bytes[(int)(packetsCount * PacketData.CONTENT_SIZE_MAX)..]);

            packetDatas.Add(packetData);
            packetDatasSize += packetData.Size;
        }

        HeaderMetadata headerMetadata = new(guid, type, packetDatasSize);
        PacketMetadata packetMetadata = new(headerMetadata);

        return new(packetMetadata, packetDatas);
    }

    public static MessageDisassembled FromMessage(Message.Message message)
    {
        uint bytesSize = 0;
        message.PacketDatas.ForEach(packetData => bytesSize += (uint)packetData.Content.Length);
        byte[] bytes = new byte[bytesSize];

        foreach (var packetData in message.PacketDatas)
        {
            packetData.Content.CopyTo(bytes, packetData.Header.Index * PacketData.CONTENT_SIZE_MAX);
        }

        return new(message.PacketMetadata.Header.GUID, message.PacketMetadata.Header.Type, bytes);
    }
}
}
