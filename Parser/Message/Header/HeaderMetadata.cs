using System.Diagnostics;

namespace Parser.Message.Header
{
public class HeaderMetadata
{
    public const uint SIZE = Utility.GUID_SIZE + sizeof(Message.Type) + sizeof(uint) + sizeof(bool);

    public Guid GUID { get; } = Guid.Empty;
    public Message.Type Type { get; } = Message.Type.UNKNOWN;
    // Size of the packets of data of the message
    public uint Size { get; } = 0;
    // the stream of bytes is fragmented in packets of data or not
    public bool Fragmented { get; } = false;

    public HeaderMetadata(Guid guid, Message.Type type, uint size, bool fragmented = false)
    {
        Debug.Assert(Message.Type.UNKNOWN < type &&
                     type < Enum.GetValues(typeof(Message.Type)).Cast<Message.Type>().Max());
        Type = type;

        GUID = guid;
        Size = size;
        Fragmented = fragmented;
    }

    public override string ToString()
    {
        string headerAsString = "";

        headerAsString += GUID.ToString();
        headerAsString += "|";
        headerAsString += Type.ToString();
        headerAsString += "|";
        headerAsString += Size.ToString();
        headerAsString += "|";
        headerAsString += Fragmented.ToString();

        return headerAsString;
    }

    public static bool operator ==(HeaderMetadata left, HeaderMetadata right)
    {
        return left.GUID == right.GUID && left.Type == right.Type && left.Size == right.Size &&
               left.Fragmented == right.Fragmented;
    }

    public static bool operator !=(HeaderMetadata left, HeaderMetadata right)
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

        return this == (HeaderMetadata)obj;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
}
