using Parser.Message;

using Networking.Context;

using Networking.TCP.Client;

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
                if (context.Type == Message.Type.ERROR)
                {
                    return;
                }

                string endPointRemote = "0.0.0.0";
                if (client.EndPointRemote != null)
                {
                    endPointRemote = client.EndPointRemote.Address.ToString();
                }
                context.IP = endPointRemote;

                client.Send(Dispatcher.Dispatch(context, client),
                            _ =>
                            {},
                            (contextProgress) => Dispatcher.Dispatch(contextProgress));
            },
            (contextProgress) => Dispatcher.Dispatch(contextProgress));
    }
}
}
