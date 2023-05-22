﻿using Parser.Message;

using Networking.Context;

using Networking.TCP.Client;

namespace Networking.Protocol
{
public class ProtocolDiscover : IProtocol
{
    readonly TCPClient? client;

    public ProtocolDiscover(IContextHandler contextHandler, TCPClient? client = null) : base(contextHandler)
    {
        ContextHandler = contextHandler;
        this.client = client;
    }

    public override IContext Exchange(IContext context)
    {
        Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (context.Type == Message.Type.REQUEST)
        {
            return new ContextDiscover(client.EndPointLocal.ToString(), Environment.MachineName);
        }
        else
        {
            ContextHandler.OnDiscover((ContextDiscover)context);
            return IContext.CreateACK();
        }
    }
}
}
