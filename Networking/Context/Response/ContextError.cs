using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context.Response
{
public class ContextError : IContext
{
    public string Message { get; set; }

    public ContextError(string message = "") : base(Parser.Message.Message.Type.ERROR, Guid.NewGuid())
    {
        Message = message;
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
