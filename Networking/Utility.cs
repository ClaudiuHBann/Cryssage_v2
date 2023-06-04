using System.Text;

namespace Networking
{
public class Utility
{
    public static readonly Encoding ENCODING_DEFAULT = Encoding.Unicode;

    public const ushort PORT_UDP_BROADCAST_CLIENT = 32404;
    public const ushort PORT_UDP_BROADCAST_SERVER = 32405;
    public const ushort PORT_TCP = 32406;

    public const uint FILE_CHUNK_SIZE = 8 * 1024 * 1024;

    public const ushort SECOND = 1000;
    // we broadcast and we wait 4 seconds
    // we update the GUI and wait 1 second
    public const ushort DELAY_BROADCAST_PROCESS_START = 4 * SECOND;
    public const ushort DELAY_BROADCAST_PROCESS_STEP = 1 * SECOND;
}
}
