using Parser.Message;
using Networking.Context.Interface;

namespace Networking.TCP.Client
{
public class ClientProcessor
{
    public ClientDispatcher Dispatcher;

    public ClientProcessor(ClientDispatcher clientDispatcher)
    {
        Dispatcher = clientDispatcher;
    }

    public void ProcessSend(TCPClient client, IContext context)
    {
        // we send the context and get the next context from the dispatcher
        client.Send(context,
                    contextProgress =>
                    {
                        Dispatcher.Dispatch(contextProgress);

                        client.Receive((context) =>
                                       {
                                           if (context.Type == Message.Type.ERROR)
                                           {
                                               return;
                                           }

                                           ProcessSend(client, Dispatcher.Dispatch(context));
                                       });
                    },
                    contextProgress => Dispatcher.Dispatch(contextProgress));
    }

    public void ProcessResponse(TCPClient client, ContextRequest contextRequest)
    {
        // we get the context from the dispatcher and send it
        client.Send(Dispatcher.DispatchResponse(contextRequest),
                    contextProgress =>
                    {
                        Dispatcher.DispatchResponse(contextProgress);

                        client.Receive(context =>
                                       {
                                           if (context.Type == Message.Type.ERROR)
                                           {
                                               return;
                                           }

                                           ProcessResponse(client, contextRequest);
                                       });
                    },
                    contextProgress => Dispatcher.DispatchResponse(contextProgress));
    }
}
}
