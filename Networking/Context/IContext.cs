using Parser.Message;

using System.Net;
using Networking.TCP.Client;

using System.Security.Cryptography;

namespace Networking.Context
{
public class IContext
{
    // the type of the context operation is the same as the message type
    public Message.Type Type { get; set; } = Message.Type.UNKNOWN;
    // the guid of the message or a new GUID
    public Guid GUID { get; set; } = Guid.NewGuid();
    // GUID of the client created from it's IP
    public Guid GUIDChat { get; set; }

    public IContext(Message.Type type, TCPClient client, Guid? guid = null)
    {
        Type = type;

        if (client.GetEndpointRemote() is IPEndPoint endpointRemoteIP)
        {
            GUIDChat = ToGUID(endpointRemoteIP.Address.ToString());
        }
        else
        {
            GUIDChat = ToGUID("0.0.0.0");
        }

        if (guid != null)
        {
            GUID = (Guid)guid;
        }
    }

    static Guid ToGUID(string data)
    {
        var dataBytes = Utility.ENCODING_DEFAULT.GetBytes(data);
        var guidBytes = SHA256.HashData(dataBytes);
        return new Guid(guidBytes);
    }
}
}
