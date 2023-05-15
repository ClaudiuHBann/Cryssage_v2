using Parser.Message.Packet;

namespace Parser.Message
{
public class Message
{
    public enum Type : byte
    {
        UNKNOWN,      // unknown message
        DISCOVER,     // discover clients message
        TEXT,         // text message
        FILE_INFO,    // file info message
        FILE_REQUEST, // file request to download message
        FILE_DATA,    // file data from the file request message
        COUNT         // used to get the size of the enum
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
