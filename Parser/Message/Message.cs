using Parser.Message.Packet;

namespace Parser.Message
{
public class Message
{
    public enum Type : byte
    {
        UNKNOWN,
        DISCOVER,
        TEXT,
        FILE,
        COUNT
    }

    public PacketMetadata PacketMetadata { get; }
    public List<PacketData> PacketDatas { get; }

    public Message(PacketMetadata packetMetadata, List<PacketData> packetDatas)
    {
        PacketMetadata = packetMetadata;
        PacketDatas = packetDatas;
    }
}
}
