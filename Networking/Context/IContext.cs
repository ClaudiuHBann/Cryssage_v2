using Parser.Message;
using Newtonsoft.Json;

using Networking.Context.File;

namespace Networking.Context
{
public class IContext
{
    // the type of the context is the same as the message type
    public Message.Type Type { get; set; } = Message.Type.UNKNOWN;
    // the guid of the message
    public Guid GUID { get; set; } = Guid.Empty;

    public IContext(Message.Type type, Guid guid)
    {
        Type = type;
        GUID = guid;
    }

    public static IContext? CreateContext(Message.Type type, Guid guid, byte[] stream)
    {
        switch (type)
        {
        case Message.Type.DISCOVER:
            return JsonConvert.DeserializeObject<ContextDiscover>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.TEXT:
            return JsonConvert.DeserializeObject<ContextText>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_INFO:
            return JsonConvert.DeserializeObject<ContextFileInfo>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_REQUEST:
            return JsonConvert.DeserializeObject<ContextFileRequest>(Utility.ENCODING_DEFAULT.GetString(stream));
        case Message.Type.FILE_DATA:
            return new ContextFileData(stream, guid);
        }

        return null;
    }

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
        case Message.Type.FILE_REQUEST:
            return Utility.ENCODING_DEFAULT.GetBytes(JsonConvert.SerializeObject((ContextFileRequest)this));
        case Message.Type.FILE_DATA:
            return ((ContextFileData)this).Stream;
        }

        return Array.Empty<byte>();
    }
}
}
