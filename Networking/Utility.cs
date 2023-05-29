﻿using System.Text;

namespace Networking
{
public class Utility
{
    public static readonly Encoding ENCODING_DEFAULT = Encoding.Unicode;

    public const ushort PORT_UDP_BROADCAST = 32405;
    public const ushort PORT_TCP = 32406;

    public const uint FILE_CHUNK_SIZE = 8 * 1024 * 1024;
}
}
