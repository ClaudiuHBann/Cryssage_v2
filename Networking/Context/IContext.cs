using Parser.Message;
using Newtonsoft.Json;

using Networking.Context.File;

namespace Networking.Context
{
// Base class for every receive context and that class used for sending context
public class IContext
{
    // this is set when a context is received in the server processor
    public string IP { get; set; } = "0.0.0.0";
    // the type of the context is the same as the message type
    public Message.Type Type { get; set; } = Message.Type.UNKNOWN;
    // the guid of the message
    public Guid GUID { get; set; } = Guid.Empty;

    // Use implicitly by derived classes
    protected IContext(Message.Type type, Guid guid)
    {
        Type = type;
        GUID = guid;
    }

    // used for real data
    public static IContext? Create(Message.Type type, Guid guid, byte[] stream)
    {
        switch (type)
        {
        case Message.Type.DISCOVER:
            return JsonConvert.DeserializeObject<ContextDiscover>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.TEXT:
            return JsonConvert.DeserializeObject<ContextText>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_INFO:
            return JsonConvert.DeserializeObject<ContextFileInfo>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.REQUEST:
            return JsonConvert.DeserializeObject<ContextRequest>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_DATA:
            return new ContextFileData(stream, guid);
        }

        return null;
    }

    // Used for progress context
    public static ContextProgress CreateProgress(Guid guid, uint total, ContextProgress.Type_ type) => new(type, total,
                                                                                                           guid);

    // Used for error context
    public static IContext CreateError() => new(Message.Type.ERROR, Guid.NewGuid());

    // Used for ack context
    public static IContext CreateACK() => new(Message.Type.ACK, Guid.NewGuid());

    public byte[] ToStream()
    {
        switch (Type)
        {
        case Message.Type.DISCOVER:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextDiscover)this));
        case Message.Type.TEXT:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextText)this));
        case Message.Type.FILE_INFO:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextFileInfo)this));
        case Message.Type.REQUEST:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextRequest)this));
        case Message.Type.FILE_DATA:
            return ((ContextFileData)this).Stream;
        case Message.Type.ERROR:
        case Message.Type.ACK:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));
        }

        return Array.Empty<byte>();
    }
}
}
