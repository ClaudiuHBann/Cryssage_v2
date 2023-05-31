using Parser.Message;

using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context.Response
{
public class ContextEOS : IContext
{
    public ContextEOS() : base(Message.Type.EOS, Guid.NewGuid())
    {
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
