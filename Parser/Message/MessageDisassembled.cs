namespace Parser.Message
{
class MessageDisassembled
{
    public Guid GUID { get; set; }
    public Message.Type Type { get; set; }
    public byte[]? Stream { get; set; }

    public MessageDisassembled(Guid guid, Message.Type type, byte[]? stream)
    {
        GUID = guid;
        Type = type;
        Stream = stream;
    }
}
}
