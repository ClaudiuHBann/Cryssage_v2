using Parser.Message;

using Networking.TCP.Client;
using Networking.Context.Interface;

namespace Networking.Manager
{
public class ManagerConnection
{
    readonly ClientProcessor processor;

    public ManagerConnection(ClientProcessor processor)
    {
        this.processor = processor;
    }

    // used for sending request or text of things like that
    public void Send(string ip, IContext context)
    {
        TCPClient client = new();
        client.Connect(ip, Utility.PORT_TCP,
                       (args) =>
                       {
                           if (!args.Connected)
                           {
                               return;
                           }

                           processor.ProcessSend(client, context);
                       });
    }

    // this is used only by the server to respond to the requests
    public void Respond(string ip, ContextRequest contextRequest)
    {
        TCPClient client = new();
        client.Connect(ip, Utility.PORT_TCP,
                       (args) =>
                       {
                           if (!args.Connected)
                           {
                               return;
                           }

                           processor.ProcessResponse(client, contextRequest);
                       });
    }
}
}
