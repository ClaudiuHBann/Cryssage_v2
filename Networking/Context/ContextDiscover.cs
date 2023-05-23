using Parser.Message;

namespace Networking.Context
{
public class ContextDiscover : IContext
{
    public string Name { get; set; }

    public ContextDiscover(string name) : base(Message.Type.DISCOVER, Guid.NewGuid())
    {
        Name = name;
    }
}
}
