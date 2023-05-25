using Parser.Message.Packet;

namespace Parser.Message
{
public class Message
{
    public enum Type : byte
    {
        UNKNOWN,   // unknown message
        ERROR,     // error message
        ACK,       // ack message sent when received message was valid
        REQUEST,   // message for request anything
        PING,      // message for broadcast
        DISCOVER,  // discover clients message
        TEXT,      // text message
        FILE_INFO, // file info message
        FILE,      // file request to download message
        FILE_DATA, // file data from the file request message
        FILE_EOF,  // file finish sending

        // not a message type but a context type
        // exists here to make things nicer and easier
        PROGRESS,

        COUNT // used to get the size of the enum
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
