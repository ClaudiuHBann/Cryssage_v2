using Parser.Message;

using Networking.TCP.Client;

namespace Networking.Context.File
{
public class ContextFileRequest : IContextFile
{
    public ContextFileRequest(TCPClient client, string path, Guid? guid = null)
        : base(Message.Type.FILE_REQUEST, client, path, guid)
    {
    }
}
}
