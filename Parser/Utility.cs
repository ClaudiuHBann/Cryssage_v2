using Parser.Message.Header;

namespace Parser
{
class Utility
{
    public static readonly uint GUID_SIZE = 16;

    public static readonly uint PACKET_DATA_SIZE_MAX = 8192;
    public static readonly uint PACKET_DATA_CONTENT_SIZE_MAX = PACKET_DATA_SIZE_MAX - HeaderData.SIZE;
}
}
