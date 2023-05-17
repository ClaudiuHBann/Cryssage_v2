using Parser.Message;

namespace Networking.Context.File
{
public class ContextFileData : IContext
{
    public byte[] Stream { get; set; }

    public ContextFileData(byte[] stream, Guid guid) : base(Message.Type.FILE_DATA, guid)
    {
        Stream = stream;
    }
}
}
