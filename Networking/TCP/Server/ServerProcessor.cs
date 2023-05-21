using Parser.Message;

using Networking.TCP.Client;

namespace Networking.TCP.Server
{
public class ServerProcessor
{
    readonly ServerDispatcher dispatcher;

    public ServerProcessor(ServerDispatcher serverDispatcher)
    {
        dispatcher = serverDispatcher;
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

                client.Send(dispatcher.Dispatch(context), (_) => Process(client));
            },
            (contextProgress) => dispatcher.Dispatch(contextProgress));
    }
}
}
