using Parser.Message;

using Networking.TCP.Client;

namespace Networking.Context
{
    public class ContextText : IContext
    {
        public string Text { get; set; }

        public ContextText(TCPClient client, string text, Guid? guid = null)
            : base(Message.Type.TEXT, client, guid)
        {
            Text = text;
        }
    }
}
