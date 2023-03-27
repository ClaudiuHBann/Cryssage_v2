namespace Parser.Message
{
class MessageDisassembled
{
    public Guid GuidGuid { get; set; }
    public Message.Type Type { get; set; }
    public byte[]? Stream { get; set; }
}
}
