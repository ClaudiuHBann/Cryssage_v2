using Parser.Message;

using Networking.Context.Interface;

namespace Networking.Context
{
public class ContextFileInfo : IContext
{
    // file path
    public string Path { get; set; }
    // file size in bytes
    public uint Size { get; set; }
    public DateTime DateTime { get; set; }

    public ContextFileInfo(string path, uint size, DateTime? dateTime = null)
        : base(Message.Type.FILE_INFO, Guid.NewGuid())
    {
        Path = path;
        Size = size;
        DateTime = dateTime ?? DateTime.UtcNow;
    }
}
}
