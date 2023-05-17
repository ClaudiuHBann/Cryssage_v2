using Parser.Message;

namespace Networking.Context.File
{
public class ContextFileInfo : IContext
{
    // file name with extension
    public string Name { get; set; } = "";
    // file size in bytes
    public uint Size { get; set; } = 0;
    public uint Timestamp { get; set; } = 0;

    public ContextFileInfo(string name, uint size, uint timestamp, Guid guid) : base(Message.Type.FILE_INFO, guid)
    {
        Name = name;
        Size = size;
        Timestamp = timestamp;
    }
}
}
