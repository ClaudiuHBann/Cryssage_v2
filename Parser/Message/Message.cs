using Parser.Message.Packet;

namespace Parser.Message
{
class Message
{
    public enum Type : byte
    {
        NONE,
        PING,
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
