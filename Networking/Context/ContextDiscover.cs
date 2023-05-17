using Parser.Message;

namespace Networking.Context
{
public class ContextDiscover : IContext
{
    public string IP { get; set; } = "0.0.0.0";
    public ushort Port { get; set; } = 0;

    public ContextDiscover(string ip, ushort port, Guid guid) : base(Message.Type.DISCOVER, guid)
    {
        IP = ip;
        Port = port;
    }
}
}
