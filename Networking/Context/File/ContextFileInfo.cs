using Parser.Message;

namespace Networking.Context.File
{
public class ContextFileInfo : IContext
{
    // file name with extension
    public string Name { get; set; }
    // file size in bytes
    public uint Size { get; set; }
    public DateTime DateTime { get; set; }

    public ContextFileInfo(string name, uint size, DateTime? dateTime = null)
        : base(Message.Type.FILE_INFO, Guid.NewGuid())
    {
        Name = name;
        Size = size;
        DateTime = dateTime ?? DateTime.UtcNow;
    }
}
}
