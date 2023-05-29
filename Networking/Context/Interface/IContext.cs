using Parser.Message;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Networking.Context.File;
using Networking.Context.Request;
using Networking.Context.Discover;

namespace Networking.Context.Interface
{
// Base class for every receive context and that class used for sending context
public class IContext
{
    // this is set when a context is received in the server processor
    public string IP { get; set; } = "127.0.0.1";
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
        case Message.Type.REQUEST: {
            var jsonStr = Utility.ENCODING_DEFAULT.GetString(stream);
            var jsonObject = JObject.Parse(jsonStr);

            switch (jsonObject["TypeRequest"]?.Value<Message.Type>())
            {
            case Message.Type.DISCOVER:
                return JsonConvert.DeserializeObject<ContextDiscoverRequest>(jsonStr);
            case Message.Type.FILE:
                return JsonConvert.DeserializeObject<ContextFileRequest>(jsonStr);

            default:
                return null;
            }
        }

        case Message.Type.RESPONSE: {
            var jsonStr = Utility.ENCODING_DEFAULT.GetString(stream);
            var jsonObject = JObject.Parse(jsonStr);

            switch (jsonObject["TypeResponse"]?.Value<Message.Type>())
            {
            case Message.Type.DISCOVER:
                return JsonConvert.DeserializeObject<ContextDiscover>(Utility.ENCODING_DEFAULT.GetString(stream));

            default:
                return null;
            }
        }

        case Message.Type.FILE_DATA:
            return new ContextFileData(stream, guid);

        case Message.Type.TEXT:
            return JsonConvert.DeserializeObject<ContextText>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_INFO:
            return JsonConvert.DeserializeObject<ContextFileInfo>(Utility.ENCODING_DEFAULT.GetString(stream));

        default:
            return null;
        }
    }

    // Used for progress context
    public static ContextProgress CreateProgress(Guid guid, uint total, ContextProgress.Type_ type) => new(type, total,
                                                                                                           guid);

    // Used for error context
    public static IContext CreateError() => new(Message.Type.ERROR, Guid.NewGuid());

    // Used for ack context
    public static IContext CreateACK() => new(Message.Type.ACK, Guid.NewGuid());

    // Used for end of stream context
    public static IContext CreateEOS() => new(Message.Type.EOS, Guid.NewGuid());

    public byte[] ToStream()
    {
        switch (Type)
        {
        case Message.Type.REQUEST:
            switch (((ContextRequest)this).TypeRequest)
            {
            case Message.Type.DISCOVER:
                return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextDiscoverRequest)this));
            case Message.Type.FILE:
                return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextFileRequest)this));

            default:
                return Array.Empty<byte>();
            }

        case Message.Type.RESPONSE:
            switch (((ContextResponse)this).TypeRespond)
            {
            case Message.Type.DISCOVER:
                return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextDiscover)this));

            default:
                return Array.Empty<byte>();
            }
        case Message.Type.FILE_DATA:
            return ((ContextFileData)this).Stream;

        case Message.Type.TEXT:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextText)this));
        case Message.Type.FILE_INFO:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextFileInfo)this));

        case Message.Type.ACK:
        case Message.Type.ERROR:
        case Message.Type.EOS:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject(this));

        default:
            return Array.Empty<byte>();
        }
    }
}
}
