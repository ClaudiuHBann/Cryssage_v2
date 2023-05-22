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
        Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

        client.Receive(
            (context) =>
            {
                if (context.Type == Message.Type.ERROR)
                {
                    return;
                }

                client.Send(Dispatcher.Dispatch(context, client),
                            _ =>
                            {},
                            (contextProgress) => Dispatcher.Dispatch(contextProgress));
            },
            (contextProgress) => Dispatcher.Dispatch(contextProgress));
    }
}
}
