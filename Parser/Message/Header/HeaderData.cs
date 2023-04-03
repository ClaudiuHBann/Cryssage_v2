using Parser.Message.Packet;

namespace Parser.Message.Header
{
public class HeaderData
{
    public const uint SIZE = Utility.GUID_SIZE + sizeof(uint);

    public Guid GUID { get; } = Guid.Empty;
    public uint Index { get; } = 0;

    public HeaderData(Guid guid, uint index)
    {
        GUID = guid;
        Index = index;
    }

    public override string ToString()
    {
        string headerAsString = "";

        headerAsString += GUID.ToString();
        headerAsString += "|";
        headerAsString += Index.ToString();

        return headerAsString;
    }

    public static bool operator ==(HeaderData left, HeaderData right)
    {
        return left.GUID == right.GUID && left.Index == right.Index;
    }

    public static bool operator !=(HeaderData left, HeaderData right)
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

        return this == (HeaderData)obj;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
}
