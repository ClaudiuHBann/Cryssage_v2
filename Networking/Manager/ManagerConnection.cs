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

    public void Send(string ip, IContext context)
    {
        // sent by the server and it's handled by the respond logic
        if (context.Type == Message.Type.REQUEST)
        {
            Respond(ip, (ContextRequest)context);
            return;
        }

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
    void Respond(string ip, ContextRequest contextRequest)
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
