using System.Diagnostics;

using Parser.Message.Header;

namespace Parser.Message.Packet
{
class PacketData
{
    public static readonly uint SIZE_MAX = Utility.PACKET_DATA_SIZE_MAX;
    public static readonly uint CONTENT_SIZE_MAX = Utility.PACKET_DATA_CONTENT_SIZE_MAX;

    public HeaderData Header { get; }
    public byte[] Content { get; }

    public uint Size => HeaderData.SIZE + (uint)Content.Length;

    public PacketData(HeaderData header, byte[] content)
    {
        Debug.Assert(content.Length <= CONTENT_SIZE_MAX);

        Header = header;
        Content = content;
    }
}
}
