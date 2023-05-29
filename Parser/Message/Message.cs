using Parser.Message.Packet;

namespace Parser.Message
{
public class Message
{
    public enum Type : byte
    {
        UNKNOWN, // unknown message

        ACK,   // ack message sent when received message was valid
        ERROR, // error message
        EOS,   // end of stream message that is sent when a socket conversation ended

        PING, // message for broadcast

        REQUEST,   // message for requesting anything
        RESPONSE,  // message for responding to anything
        DISCOVER,  // discover clients message
        FILE,      // file request to download message
        FILE_DATA, // file data from the file request message

        TEXT,      // text message
        FILE_INFO, // file info message

        // not a message type but a context type
        // exists here to make things nicer and easier
        PROGRESS
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
