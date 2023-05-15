using Parser.Message;

using Networking.TCP.Client;

namespace Networking.Context.File
{
public class IContextFile : IContext
{
    // remote file path (this is like a GUID)
    public string Path { get; set; }

    public IContextFile(Message.Type type, TCPClient client, string path, Guid? guid = null) : base(type, client, guid)
    {
        Path = path;
    }
}
}
