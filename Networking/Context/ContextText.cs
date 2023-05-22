using Parser.Message;

namespace Networking.Context
{
public class ContextText : IContext
{
    public string Text { get; set; } = "";
    public uint Timestamp { get; set; } = 0;

    public ContextText(string text, uint timestamp) : base(Message.Type.TEXT, Guid.NewGuid())
    {
        Text = text;
        Timestamp = timestamp;
    }
}
}
