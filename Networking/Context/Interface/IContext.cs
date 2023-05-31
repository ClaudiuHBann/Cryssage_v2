using Parser.Message;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Networking.Context.File;
using Networking.Context.Request;
using Networking.Context.Discover;
using Networking.Context.Response;

namespace Networking.Context.Interface
{
// Base class for every receive context and that class used for sending context
public abstract class IContext
{
    [JsonIgnore]
    // this is set when a context is received in the server processor
    public string IP { get; set; } = "127.0.0.1";

    // the type of the context is the same as the message type
    public Message.Type Type { get; set; } = Message.Type.UNKNOWN;

    // the guid of the message
    public Guid GUID { get; set; } = Guid.Empty;

    [JsonIgnore]
    // the request or context to send was handled
    public bool Responded { get; set; } = false;

    [JsonIgnore]
    public bool HasProgress { get; set; } = false;

    // used by json deserializer
    public IContext()
    {
    }

    // Use implicitly by derived classes
    protected IContext(Message.Type type, Guid guid, bool hasProgress = false)
    {
        Type = type;
        GUID = guid;
        HasProgress = hasProgress;
    }

    public abstract byte[] ToStream();

    // not an abstract method because c#'s 'this' is readonly and we cannot initialize ourself in the constructor
    public static IContext? Create(Message.Type type, Guid guid, byte[] stream)
    {
        switch (type)
        {
        case Message.Type.REQUEST: {
            var jsonStr = Utility.ENCODING_DEFAULT.GetString(stream);
            var jsonObject = JObject.Parse(jsonStr);

            switch ((Message.Type?)jsonObject[nameof(ContextRequest.TypeRequest)]?.Value<long>())
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

            switch ((Message.Type?)jsonObject[nameof(ContextResponse.TypeResponse)]?.Value<long>())
            {
            case Message.Type.DISCOVER:
                return JsonConvert.DeserializeObject<ContextDiscover>(Utility.ENCODING_DEFAULT.GetString(stream));

            default:
                return null;
            }
        }
            // the file response is just the stream of data for speed
        case Message.Type.FILE_DATA:
            return new ContextFileData(stream, guid);

            // simple messages
        case Message.Type.TEXT:
            return JsonConvert.DeserializeObject<ContextText>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_INFO:
            return JsonConvert.DeserializeObject<ContextFileInfo>(Utility.ENCODING_DEFAULT.GetString(stream));

            // responses
        case Message.Type.ACK:
            return JsonConvert.DeserializeObject<ContextACK>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.ERROR:
            return JsonConvert.DeserializeObject<ContextError>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.EOS:
            return JsonConvert.DeserializeObject<ContextEOS>(Utility.ENCODING_DEFAULT.GetString(stream));

        default:
            return null;
        }
    }
}
}
