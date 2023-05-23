using Networking.Context;

using Networking.TCP.Client;
using Networking.TCP.Server;

namespace Networking.Manager
{
public class ManagerConnection
{
    readonly ServerProcessor processor;

    public ManagerConnection(ServerProcessor processor)
    {
        this.processor = processor;
    }

    public void CreateConnectionAndSend(string ip, IContext context)
    {
        TCPClient client = new();
        client.Connect(ip, Utility.PORT_TCP,
                       (args) =>
                       {
                           if (!args.Connected)
                           {
                               return;
                           }

                           processor.Process(client, context);
                       });
    }
}
}
