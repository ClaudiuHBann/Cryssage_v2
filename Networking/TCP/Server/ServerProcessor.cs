using Parser.Message;

using Networking.Context;

using Networking.TCP.Client;
using System.Net;

namespace Networking.TCP.Server
{
public class ServerProcessor
{
    public ServerDispatcher Dispatcher;

    public ServerProcessor(ServerDispatcher serverDispatcher)
    {
        Dispatcher = serverDispatcher;
    }

    public void Process(TCPClient client, IContext context)
    {
        client.Send(context, (context) => Process(client), (contextProgress) => Dispatcher.Dispatch(contextProgress));
    }

    public void Process(TCPClient client)
    {
        client.Receive(
            (context) =>
            {
                if (context.Type == Message.Type.ERROR || context.Type == Message.Type.FILE_EOF)
                {
                    return;
                }

                string endPointRemote = "127.0.0.1";
                if (client.EndPointRemote != null &&
                    client.EndPointRemote.Address.ToString() != IPAddress.IPv6Loopback.ToString())
                {
                    endPointRemote = client.EndPointRemote.Address.ToString();
                }
                context.IP = endPointRemote;

                client.Send(Dispatcher.Dispatch(context),
                            _ => Process(client), (contextProgress) => Dispatcher.Dispatch(contextProgress));
            },
            (contextProgress) => Dispatcher.Dispatch(contextProgress));
    }
}
}
