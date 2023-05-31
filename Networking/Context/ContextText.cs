using Parser.Message;

using Newtonsoft.Json;

using Networking.Context.Interface;

namespace Networking.Context
{
public class ContextText : IContext
{
    public string Text { get; set; }
    public DateTime DateTime { get; set; }

    public ContextText(string text, DateTime? dateTime = null) : base(Message.Type.TEXT, Guid.NewGuid(), true)
    {
        Text = text;
        DateTime = dateTime ?? DateTime.UtcNow;
    }

    public override byte[] ToStream() => Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
}
}
