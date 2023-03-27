namespace Parser.Message.Header
{
class HeaderData
{
    public static readonly int SIZE = Utility.GUID_SIZE + sizeof(uint);

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
        headerAsString += Utility.HEADER_STRING_DELIMITATOR;
        headerAsString += Index.ToString();

        return headerAsString;
    }
}
}
