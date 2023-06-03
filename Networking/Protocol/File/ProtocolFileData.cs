using Networking.Manager;

using Networking.Context;
using Networking.Context.File;
using Networking.Context.Response;
using Networking.Context.Interface;

namespace Networking.Protocol.File
{
    public class ProtocolFileData : IProtocol
{
    readonly ManagerFileTransfer managerFileTransfer;

    public ProtocolFileData(IContextHandler contextHandler, ManagerFileTransfer managerFileTransfer)
        : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.managerFileTransfer = managerFileTransfer;
    }

    public override IContext GetNextContext(IContext context)
    {
        var (stream, fileInfo) = managerFileTransfer.Read(context.GUID);
        if (fileInfo != null)
        {
            var contextProgress =
                new ContextProgress(ContextProgress.Type_.SEND, fileInfo.Size, context.GUID) { IP = context.IP };
            contextProgress.SetPercentage(fileInfo.Index);
            ContextHandler.OnSendProgress(contextProgress);
        }

        return stream != null ? new ContextFileData(stream, context.GUID) : new ContextEOS();
    }

    public override IContext Exchange(IContext context)
    {
        var contextFileData = (ContextFileData)context;

        var fileInfo = managerFileTransfer.Write(contextFileData.GUID, contextFileData.Stream);
        if (fileInfo != null)
        {
            var contextProgress =
                new ContextProgress(ContextProgress.Type_.RECEIVE, fileInfo.Size, context.GUID) { IP = context.IP };
            contextProgress.SetPercentage(fileInfo.Index);
            ContextHandler.OnReceiveProgress(contextProgress);
        }

        return fileInfo != null ? new ContextACK() : new ContextError();
    }
}
}
