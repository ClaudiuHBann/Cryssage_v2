using Parser.Message;

using Networking.TCP.Client;

namespace Networking.Context.File
{
public class ContextFileData : IContextFile
{
    public byte[] Data { get; set; }

    public ContextFileData(TCPClient client, string path, byte[] data, Guid? guid = null)
        : base(Message.Type.FILE_DATA, client, path, guid)
    {
        Data = data;
    }
}
}
