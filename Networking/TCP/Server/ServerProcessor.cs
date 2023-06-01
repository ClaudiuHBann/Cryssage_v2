using Parser.Message;

using Networking.TCP.Client;
using Networking.Context.Interface;

namespace Networking.TCP.Server
{
public class ServerProcessor
{
    public ServerDispatcher Dispatcher;

    public ServerProcessor(ServerDispatcher serverDispatcher)
    {
        Dispatcher = serverDispatcher;
    }

    static string GetClientEndPointRemote(TCPClient client)
    {
        string endPointRemote = "127.0.0.1";
        if (client.EndPointRemote != null && !client.EndPointRemote.Address.ToString().Contains("127.0.0.1"))
        {
            endPointRemote = client.EndPointRemote.Address.ToString();
        }

        return endPointRemote;
    }

    public void Process(TCPClient client)
    {
        client.Receive(context =>
                       {
                           if (context.Type == Message.Type.ERROR || context.Type == Message.Type.EOS)
                           {
                               return;
                           }

                           context.IP = GetClientEndPointRemote(client);
                           client.Send(Dispatcher.Dispatch(context),
                                       _ => Process(client));
                       });
    }
}
}
