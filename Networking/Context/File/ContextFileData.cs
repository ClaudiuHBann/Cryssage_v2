using Parser.Message;

using Networking.Context.Interface;

namespace Networking.Context.File
{
// this should have been a context response for the pattern
// the file context translates to a stream of data bytes not to a json
// and for simplicity we choose to interpret this type of message as a regular message
public class ContextFileData : IContext
{
    public byte[] Stream { get; set; }

    public ContextFileData(byte[] stream, Guid guid) : base(Message.Type.FILE_DATA, guid)
    {
        Stream = stream;
    }

    public override byte[] ToStream() => Stream;
}
}
