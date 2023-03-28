using Parser.Message.Header;

namespace Parser.Message.Packet
{
public class PacketMetadata
{
    public HeaderMetadata Header { get; }

    public PacketMetadata(HeaderMetadata header)
    {
        Header = header;
    }

    public static bool operator ==(PacketMetadata left, PacketMetadata right)
    {
        return left.Header == right.Header;
    }

    public static bool operator !=(PacketMetadata left, PacketMetadata right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        return this == (PacketMetadata)obj;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
}
