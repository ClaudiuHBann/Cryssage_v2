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
        var contextReponse = Dispatcher.Dispatch(context);
        client.Send(contextReponse,
                    contextProgress =>
                    {
                        // if we sent EOS stop
                        if (contextReponse.Type == Message.Type.EOS)
                        {
                            return;
                        }

                        Dispatcher.Dispatch(contextProgress);

                        client.Receive(contextR =>
                                       {
                                           if (contextR.Type == Message.Type.ERROR)
                                           {
                                               return;
                                           }

                                           ProcessSend(client, context);
                                       });
                    },
                    contextProgress => Dispatcher.Dispatch(contextProgress));
    }

    public void ProcessResponse(TCPClient client, ContextRequest contextRequest)
    {
        var contextReponse = Dispatcher.DispatchResponse(contextRequest);
        client.Send(contextReponse,
                    contextProgress =>
                    {
                        // if we sent EOS stop
                        if (contextReponse.Type == Message.Type.EOS)
                        {
                            return;
                        }

                        Dispatcher.DispatchResponse(contextProgress);

                        client.Receive(context =>
                                       {
                                           if (context.Type == Message.Type.ERROR)
                                           {
                                               return;
                                           }

                                           // process with the same context request
                                           ProcessResponse(client, contextRequest);
                                       });
                    },
                    contextProgress => Dispatcher.DispatchResponse(contextProgress));
    }
}
}
