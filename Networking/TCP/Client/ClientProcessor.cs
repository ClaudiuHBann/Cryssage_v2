using Parser.Message;

using Networking.TCP.Server;

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
        contextReponse.IP = ServerProcessor.GetClientEndPointRemote(client);
        client.Send(contextReponse,
                    contextProgress =>
                    {
                        // if we sent EOS stop
                        if (contextReponse.Type == Message.Type.EOS)
                        {
                            return;
                        }

                        contextProgress.IP = contextReponse.IP;
                        if (contextReponse.HasProgress)
                        {
                            Dispatcher.Dispatch(contextProgress);
                        }

                        client.Receive(contextR =>
                                       {
                                           if (contextR.Type == Message.Type.ERROR)
                                           {
                                               return;
                                           }

                                           ProcessSend(client, context);
                                       });
                    },
                    contextProgress =>
                    {
                        contextProgress.IP = contextReponse.IP;
                        if (contextReponse.HasProgress)
                        {
                            Dispatcher.Dispatch(contextProgress);
                        }
                    });
    }

    public void ProcessResponse(TCPClient client, ContextRequest contextRequest)
    {
        var contextReponse = Dispatcher.DispatchResponse(contextRequest);
        contextReponse.IP = ServerProcessor.GetClientEndPointRemote(client);
        client.Send(contextReponse,
                    contextProgress =>
                    {
                        // if we sent EOS stop
                        if (contextReponse.Type == Message.Type.EOS)
                        {
                            return;
                        }

                        contextProgress.IP = contextReponse.IP;
                        if (contextReponse.HasProgress)
                        {
                            Dispatcher.DispatchResponse(contextProgress);
                        }

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
                    contextProgress =>
                    {
                        contextProgress.IP = contextReponse.IP;
                        if (contextReponse.HasProgress)
                        {
                            Dispatcher.DispatchResponse(contextProgress);
                        }
                    });
    }
}
}
