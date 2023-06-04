using Parser.Message;

using System.Net;
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

    public static string GetClientEndPointRemote(TCPClient client)
    {
        string endPointRemote = IPAddress.Loopback.ToString();
        if (client.EndPointRemote != null)
        {
            endPointRemote = client.EndPointRemote.Address.MapToIPv4().ToString();
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
