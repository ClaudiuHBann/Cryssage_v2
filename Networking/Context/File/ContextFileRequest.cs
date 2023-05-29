using Parser.Message;

using Networking.Context.Interface;

namespace Networking.Context.File
{
public class ContextFileRequest : ContextRequest
{
    // local file path where the file will be downloaded
    public string Path { get; set; }
    // the size of the remote file
    public uint Size { get; set; }
    // the size of the local file
    public uint Index { get; set; }

    // local file path and the file guid
    public ContextFileRequest(string path, uint size, Guid guid) : base(Message.Type.FILE, guid)
    {
        Path = path;
        Size = size;
        Index = (uint) new FileInfo(path).Length;
    }
}
}
