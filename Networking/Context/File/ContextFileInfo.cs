using Parser.Message;

using Networking.TCP.Client;

namespace Networking.Context.File
{
public class ContextFileInfo : IContextFile
{
    // file name with extension
    public string Name { get; set; }
    // file size in bytes
    public uint Size { get; set; }

    public ContextFileInfo(TCPClient client, string path, uint size, Guid? guid = null)
        : base(Message.Type.FILE_INFO, client, path, guid)
    {
        Name = System.IO.Path.GetFileName(path);
        Size = size;
    }
}
}
