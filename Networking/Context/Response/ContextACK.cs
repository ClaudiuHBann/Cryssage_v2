using Parser.Message;

using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context.Response
{
public class ContextACK : IContext
{
    public ContextACK() : base(Message.Type.ACK, Guid.NewGuid())
    {
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
