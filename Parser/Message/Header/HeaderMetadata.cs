﻿using System.Diagnostics;

namespace Parser.Message.Header
{
public class HeaderMetadata
{
    public static readonly uint SIZE = Utility.GUID_SIZE + sizeof(Message.Type) + sizeof(uint);

    public Guid GUID { get; } = Guid.Empty;
    public Message.Type Type { get; } = Message.Type.NONE;
    public uint Size { get; } = 0;

    public HeaderMetadata(Guid guid, Message.Type type, uint size)
    {
        Debug.Assert(Message.Type.NONE < type && type < Message.Type.COUNT);
        Type = type;

        GUID = guid;
        Size = size;
    }

    public override string ToString()
    {
        string headerAsString = "";

        headerAsString += GUID.ToString();
        headerAsString += "|";
        headerAsString += Type.ToString();
        headerAsString += "|";
        headerAsString += Size.ToString();

        return headerAsString;
    }

    public static bool operator ==(HeaderMetadata left, HeaderMetadata right)
    {
        return left.GUID == right.GUID && left.Type == right.Type && left.Size == right.Size;
    }

    public static bool operator !=(HeaderMetadata left, HeaderMetadata right)
    {
        return !(left == right);
    }
}
}
