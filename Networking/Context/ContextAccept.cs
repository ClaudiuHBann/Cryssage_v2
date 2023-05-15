using Parser.Message;

using System.Net;
using Networking.TCP.Client;

namespace Networking.Context
{
    public class ContextAccept : IContext
    {
        public string IP { get; set; } = "0.0.0.0";

        public ContextAccept(TCPClient client, Guid? guid = null) : base(Message.Type.DISCOVER, client, guid)
        {
            if (client.GetEndpointRemote() is IPEndPoint endpointRemoteIP)
            {
                IP = endpointRemoteIP.Address.ToString();
            }
        }
    }
}
