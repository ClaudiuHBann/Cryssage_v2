using System.Collections.Concurrent;

using Networking.TCP.Client;

namespace Networking.Manager
{
public class ManagerConnection
{
    public ConcurrentDictionary<string, TCPClient> Clients = new();

    public void CreateConnections(List<string> ips) => ips.ForEach(CreateConnection);

    public void CreateConnection(string ip)
    {
        TCPClient client = new();
        client.Connect(ip, Utility.PORT_TCP,
                       (args) =>
                       {
                           if (args.Connected)
                           {
                               Clients[ip] = client;
                           }
                       });
    }
}
}
