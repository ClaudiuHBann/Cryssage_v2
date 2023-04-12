using Parser.Message;

namespace Networking.Protocol.Context.Operation
{
public class ContextOperationFile : IContextOperation
{
    public Guid GUIDFile { get; set; }
    public string Path { get; set; }

    public ContextOperationFile(Guid guidChat, Guid guidFile, string path) : base(Message.Type.FILE, guidChat)
    {
        GUIDFile = guidFile;
        Path = path;
    }
}
}
