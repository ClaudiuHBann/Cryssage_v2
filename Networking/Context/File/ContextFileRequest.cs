using Parser.Message;

namespace Networking.Context.File
{
public class ContextFileRequest : IContext
{
    public ContextFileRequest(Guid guid) : base(Message.Type.FILE_REQUEST, guid)
    {
    }
}
}
