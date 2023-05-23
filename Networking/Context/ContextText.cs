using Parser.Message;

namespace Networking.Context
{
public class ContextText : IContext
{
    public string Text { get; set; }
    public DateTime DateTime { get; set; }

    public ContextText(string text, DateTime? dateTime = null) : base(Message.Type.TEXT, Guid.NewGuid())
    {
        Text = text;
        DateTime = dateTime ?? DateTime.UtcNow;
    }
}
}
