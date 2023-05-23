using Parser.Message;

using Networking.Context;

using Networking.TCP.Client;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    readonly TCPClient? client;

    public ProtocolDiscover(IContextHandler contextHandler, TCPClient? client = null) : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.client = client;
    }

    public override IContext Exchange(IContext context)
    {
        if (context.Type == Message.Type.REQUEST)
        {
            string endPointLocal = "0.0.0.0";
            if (client != null && client.EndPointLocal != null)
            {
                endPointLocal = client.EndPointLocal.Address.ToString();
            }

            return new ContextDiscover(Environment.MachineName);
        }
        else
        {
            ContextHandler.OnDiscover((ContextDiscover)context);
            return IContext.CreateACK();
        }
    }
}
}
