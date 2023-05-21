using System.Collections.Concurrent;

using System.Net;
using Networking.TCP.Client;

namespace Networking.Manager
{
public class ManagerConnection
{
    public ConcurrentDictionary<string, TCPClient> Clients = new();

    public void CreateConnections(List<IPEndPoint> ips) => ips.ForEach(CreateConnection);

    public void CreateConnection(IPEndPoint ipEndPoint)
    {
        TCPClient client = new();
        client.Connect(ipEndPoint.Address.ToString(), Utility.PORT_TCP,
                       (args) =>
                       {
                           if (args.Connected)
                           {
                               Clients[ipEndPoint.Address.ToString()] = client;
                           }
                       });
    }
}
}
