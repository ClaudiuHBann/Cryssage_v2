using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class IContextOperationFile : IContextOperation
{
    public Guid GUIDFile { get; set; }
    public string Path { get; set; }

    public IContextOperationFile(Guid guidChat, Guid guidFile, string path) : base(Message.Type.FILE, guidChat)
    {
        GUIDFile = guidFile;
        Path = path;
    }
}
}
