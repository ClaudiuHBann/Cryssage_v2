﻿using Parser;
using Parser.Message;

using Networking.Context;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    public ProtocolDiscover(IContextHandler contextHandler) : base(contextHandler)
    {
        ContextHandler = contextHandler;
    }

    public override Message Exchange(IContext context)
    {
        return MessageManager.ToMessageAck();
    }
}
}
