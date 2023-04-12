using Parser.Message;

namespace Networking.Protocol
{
    public interface IProtocol
    {
        public Message Exchange(Message message);
    }
}
