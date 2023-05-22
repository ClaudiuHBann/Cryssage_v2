using Parser.Message;

namespace Networking.Context
{
public class ContextDiscover : IContext
{
    public string Ip { get; set; } = "0.0.0.0";
    public string Name { get; set; } = "Unknown";

    public ContextDiscover(string ip, string name) : base(Message.Type.DISCOVER, Guid.NewGuid())
    {
        Ip = ip;
        Name = name;
    }
}
}
