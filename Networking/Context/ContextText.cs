using Parser.Message;

namespace Networking.Context
{
public class ContextText : IContext
{
    public string Text { get; set; } = "";
    public uint Timestamp { get; set; } = 0;

    public ContextText(string text, uint timestamp, Guid guid) : base(Message.Type.TEXT, guid)
    {
        Text = text;
        Timestamp = timestamp;
    }
}
}
