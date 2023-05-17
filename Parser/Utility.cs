using Parser.Message.Header;

namespace Parser
{
class Utility
{
    public const uint GUID_SIZE = 16;

    public const uint PACKET_DATA_SIZE_MAX = ushort.MaxValue;
    public const uint PACKET_DATA_CONTENT_SIZE_MAX = PACKET_DATA_SIZE_MAX - HeaderData.SIZE;
}
}
