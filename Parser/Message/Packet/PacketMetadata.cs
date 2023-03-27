using Parser.Message.Header;

namespace Parser.Message.Packet
{
class PacketMetadata
{
    public HeaderMetadata Header { get; }

    public PacketMetadata(HeaderMetadata header)
    {
        Header = header;
    }
}
}
