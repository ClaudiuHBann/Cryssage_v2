﻿using Parser.Message;

using Networking.Context;

using Networking.Protocol.File;
using Networking.Protocol;

namespace Networking.TCP.Server
{
public class ServerDispatcher
{
    public IContextHandler ContextHandler { get; set; }

    public ServerDispatcher(IContextHandler contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public IContext Dispatch(IContext context)
    {
        IProtocol? protocol = null;
        switch (context.Type)
        {
        case Message.Type.DISCOVER:
            protocol = new ProtocolDiscover(ContextHandler);
            break;
        case Message.Type.TEXT:
            protocol = new ProtocolText(ContextHandler);
            break;
        case Message.Type.FILE_INFO:
            protocol = new ProtocolFileInfo(ContextHandler);
            break;
        case Message.Type.FILE_REQUEST:
            protocol = new ProtocolFileRequest(ContextHandler);
            break;
        case Message.Type.FILE_DATA:
            protocol = new ProtocolFileData(ContextHandler);
            break;
        }

        return protocol != null ? protocol.Exchange(context) : IContext.CreateError();
    }
}
}