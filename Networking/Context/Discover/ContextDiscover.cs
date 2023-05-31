using Parser.Message;

using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context.Discover
{
public class ContextDiscover : ContextResponse
{
    public string Name { get; set; }

    public ContextDiscover(string name) : base(Message.Type.DISCOVER, Guid.NewGuid())
    {
        Name = name;
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
