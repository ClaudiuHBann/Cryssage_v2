using Parser.Message;

using Networking.Context.Interface;

namespace Networking.Context.Request
{
public class ContextDiscoverRequest : ContextRequest
{
    public ContextDiscoverRequest() : base(Message.Type.DISCOVER, Guid.NewGuid())
    {
    }
}
}
